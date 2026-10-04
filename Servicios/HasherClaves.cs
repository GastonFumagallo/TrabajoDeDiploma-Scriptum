using System.Security.Cryptography;
using System.Text;

namespace Servicios
{
    /// <summary>
    /// Hash de contraseñas con PBKDF2-HMAC-SHA256, salt aleatorio por usuario y factor de trabajo alto.
    /// Formato guardado: <c>PBKDF2-SHA256$iteraciones$salt(base64)$hash(base64)</c>, así el factor de trabajo
    /// se puede subir en el futuro sin invalidar las claves existentes.
    /// <para>
    /// También verifica el formato heredado (SHA-256 hexadecimal sin salt) para migrar en el próximo login:
    /// <see cref="Verificar"/> devuelve <c>requiereRehash = true</c> y el llamador guarda el hash nuevo.
    /// </para>
    /// </summary>
    public static class HasherClaves
    {
        private const string Prefijo = "PBKDF2-SHA256";
        private const int Iteraciones = 600_000;   // recomendación OWASP para PBKDF2-HMAC-SHA256
        private const int LargoSalt = 16;
        private const int LargoHash = 32;

        // Se verifica contra este hash cuando el usuario no existe, para que la respuesta tarde lo mismo
        // y no revele qué nombres de usuario son válidos.
        private static readonly Lazy<string> HashFicticio = new(() => Hashear(Guid.NewGuid().ToString()));

        public static string Hashear(string clave)
        {
            ArgumentNullException.ThrowIfNull(clave);
            byte[] salt = RandomNumberGenerator.GetBytes(LargoSalt);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(clave, salt, Iteraciones, HashAlgorithmName.SHA256, LargoHash);
            return $"{Prefijo}${Iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool Verificar(string clave, string? hashGuardado, out bool requiereRehash)
        {
            requiereRehash = false;
            if (string.IsNullOrEmpty(clave) || string.IsNullOrEmpty(hashGuardado))
                return false;

            var partes = hashGuardado.Split('$');
            if (partes.Length == 4 && partes[0] == Prefijo && int.TryParse(partes[1], out int iteraciones))
            {
                byte[] salt, esperado;
                try
                {
                    salt = Convert.FromBase64String(partes[2]);
                    esperado = Convert.FromBase64String(partes[3]);
                }
                catch (FormatException)
                {
                    return false;
                }
                byte[] calculado = Rfc2898DeriveBytes.Pbkdf2(clave, salt, iteraciones, HashAlgorithmName.SHA256, esperado.Length);
                bool ok = CryptographicOperations.FixedTimeEquals(calculado, esperado);
                requiereRehash = ok && iteraciones < Iteraciones;
                return ok;
            }

            // Formato heredado: SHA-256 en hexadecimal, sin salt (64 caracteres).
            if (hashGuardado.Length == 64)
            {
                byte[] calculado = SHA256.HashData(Encoding.UTF8.GetBytes(clave));
                byte[] esperado;
                try { esperado = Convert.FromHexString(hashGuardado); }
                catch (FormatException) { return false; }
                bool ok = CryptographicOperations.FixedTimeEquals(calculado, esperado);
                requiereRehash = ok;
                return ok;
            }

            return false;
        }

        /// <summary>Consume el mismo tiempo que una verificación real (para usuarios inexistentes).</summary>
        public static void SimularVerificacion(string clave) => Verificar(clave, HashFicticio.Value, out _);
    }

    /// <summary>Reglas para las contraseñas que elige el usuario.</summary>
    public static class PoliticaClaves
    {
        public const int LargoMinimo = 8;

        /// <summary>Devuelve el motivo por el que la clave no es válida, o null si cumple la política.</summary>
        public static string? Validar(string clave, string? nombreUsuario)
        {
            if (string.IsNullOrEmpty(clave) || clave.Length < LargoMinimo)
                return $"La clave debe tener al menos {LargoMinimo} caracteres.";
            if (!clave.Any(char.IsLetter) || !clave.Any(char.IsDigit))
                return "La clave debe combinar letras y números.";
            if (!string.IsNullOrEmpty(nombreUsuario) && clave.Contains(nombreUsuario, StringComparison.OrdinalIgnoreCase))
                return "La clave no puede contener el nombre de usuario.";
            return null;
        }
    }
}
