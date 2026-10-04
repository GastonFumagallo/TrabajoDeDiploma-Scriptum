using System.Security.Cryptography;
using System.Text;

namespace Servicios
{
    /// <summary>
    /// Hash de claves con PBKDF2-HMAC-SHA256 (600.000 iteraciones, sal aleatoria de 16 bytes por usuario),
    /// incluido en .NET sin dependencias. Formato guardado: <c>pbkdf2-sha256$iteraciones$sal$hash</c> (Base64).
    /// Reconoce el formato heredado (SHA-256 hex sin sal) para migrarlo en el primer inicio de sesión.
    /// </summary>
    public static class HasherClaves
    {
        private const string Prefijo = "pbkdf2-sha256";
        private const int Iteraciones = 600_000;      // recomendación OWASP para PBKDF2-HMAC-SHA256
        private const int LargoSal = 16;
        private const int LargoHash = 32;
        public const int LargoMinimo = 10;

        // Sin caracteres que se confunden al dictarlos o leerlos (0/O, 1/l/I).
        private const string AlfabetoTemporal = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";

        public static string Hashear(string clave)
        {
            var sal = RandomNumberGenerator.GetBytes(LargoSal);
            var hash = Rfc2898DeriveBytes.Pbkdf2(clave, sal, Iteraciones, HashAlgorithmName.SHA256, LargoHash);
            return $"{Prefijo}${Iteraciones}${Convert.ToBase64String(sal)}${Convert.ToBase64String(hash)}";
        }

        /// <summary>Compara en tiempo constante.</summary>
        /// <param name="requiereRehash">true si la clave es correcta pero el hash está en un formato o costo viejo.</param>
        public static bool Verificar(string clave, string? almacenado, out bool requiereRehash)
        {
            requiereRehash = false;
            if (string.IsNullOrEmpty(almacenado)) return false;

            try
            {
                if (almacenado.Split('$') is [Prefijo, var textoIteraciones, var sal, var hash]
                    && int.TryParse(textoIteraciones, out var iteraciones) && iteraciones > 0)
                {
                    var esperado = Convert.FromBase64String(hash);
                    var calculado = Rfc2898DeriveBytes.Pbkdf2(clave, Convert.FromBase64String(sal), iteraciones,
                        HashAlgorithmName.SHA256, esperado.Length);
                    var ok = CryptographicOperations.FixedTimeEquals(calculado, esperado);
                    requiereRehash = ok && iteraciones < Iteraciones;
                    return ok;
                }

                // Formato heredado: SHA-256 en hexadecimal, sin sal.
                if (almacenado.Length == 64)
                {
                    var ok = CryptographicOperations.FixedTimeEquals(
                        SHA256.HashData(Encoding.UTF8.GetBytes(clave)), Convert.FromHexString(almacenado));
                    requiereRehash = ok;
                    return ok;
                }
            }
            catch (FormatException)
            {
                // Hash corrupto: se trata como clave incorrecta.
            }
            return false;
        }

        /// <summary>Clave temporal de 12 caracteres con generador criptográfico (~70 bits de entropía).</summary>
        public static string GenerarTemporal() => RandomNumberGenerator.GetString(AlfabetoTemporal, 12);

        /// <summary>Código numérico de 6 dígitos para la recuperación por mail.</summary>
        public static string GenerarCodigo() => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

        /// <summary>
        /// Reglas de fortaleza (NIST 800-63B: largo antes que composición). Devuelve el error, o null si es válida.
        /// </summary>
        public static string? ValidarFortaleza(string? clave, string? usuario)
        {
            if (string.IsNullOrEmpty(clave) || clave.Length < LargoMinimo)
                return $"La clave debe tener al menos {LargoMinimo} caracteres.";
            if (clave.Length > 128)
                return "La clave no puede superar los 128 caracteres.";
            if (!string.IsNullOrWhiteSpace(usuario) && clave.Contains(usuario.Trim(), StringComparison.OrdinalIgnoreCase))
                return "La clave no puede contener el nombre de usuario.";
            if (clave.Distinct().Count() < 4)
                return "La clave es demasiado simple: usá más caracteres distintos.";
            return null;
        }
    }
}
