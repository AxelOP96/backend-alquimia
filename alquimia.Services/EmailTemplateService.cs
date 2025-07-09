using alquimia.Services.Interfaces;

namespace alquimia.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GetDesignPDFProviderEmail(string provider, string creator)
        {
            return $@"
                <div
        style=""font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; border: 1px solid #ddd; border-radius: 8px; overflow: hidden;"">

        <!-- 🖼️ Imagotipo -->
        <div style=""text-align: center; padding: 20px; background-color: #fff;"">
            <img src=""https://res.cloudinary.com/dxnwcmh1j/image/upload/v1750382478/imagotipo_mpkd4p.png"" alt=""Alquimia Logo""
                width=""120"" style=""margin: auto;"" />
        </div>

        <!-- 📄 Cuerpo -->
        <div style=""padding: 30px; background-color: #fff;"">
            <p style=""font-size: 16px;"">Hola, <strong>{provider}</strong>.</p>

            <p style=""font-size: 15px;"">Te adjuntamos el PDF correspondiente al <strong>diseño de etiqueta</strong> de <strong>{creator}</strong></p>

            <p style=""font-size: 14px; color: #555;"">Agradecemos tu paciencia y tu interés en formar parte de nuestra comunidad.</p>

            <!-- Firma -->
            <p style=""margin-top: 40px; font-weight: bold; font-size: 15px; color: #9445b7;"">Equipo de Alquimia</p>
        </div>

        <!-- 📎 Footer -->
        <div style=""background-color: #f4f4f4; text-align: center; padding: 20px; font-size: 12px; color: #999;"">
            © 2025 Alquimia — Todos los derechos reservados
            <br /><br />
            <a href=""https://instagram.com/alquimia.frag"" target=""_blank""
                style=""text-decoration: none; display: inline-flex; align-items: center; gap: 6px; color: #9445b7; font-weight: bold; font-size: 13px;"">
                <img src=""http://upload.wikimedia.org/wikipedia/commons/a/a5/Instagram_icon.png"" width=""16"" height=""16""
                    alt=""Instagram"" style=""display: inline-block;"" />
                @alquimia.frag
            </a>
        </div>
    </div>";
        }

        public string GetPasswordResetEmail(string userName, string resetLink)
        {
            return $@"
            <div
        style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; border: 1px solid #ddd; border-radius: 8px; overflow: hidden;'>

        <!-- 🖼️ Imagotipo de Alquimia (acá lo insertás si querés) -->
        <div style='text-align: center; padding: 20px; background-color: #fff;'>
            <img src='https://res.cloudinary.com/dxnwcmh1j/image/upload/v1750382478/imagotipo_mpkd4p.png' alt='Alquimia Logo' width='120' style='margin: auto;' />
        </div>

        <!-- Cuerpo del mensaje -->
        <div style='padding: 30px; background-color: #fff;'>
            <p style='font-size: 16px;'>Hola, <strong>{userName}</strong> .</p>

            <p style='font-size: 15px;'>Recibimos una solicitud para restablecer tu contraseña. Hacé clic en el
                siguiente botón para continuar:</p>

            <p style='text-align: center; margin: 30px 0;'>
                <a href='{resetLink}'
                    style='background-color: #9445b7; color: white; padding: 12px 25px; text-decoration: none; font-size: 16px; border-radius: 6px; display: inline-block;'>
                    Restablecer contraseña
                </a>
            </p>

            <p style='font-size: 14px; color: #555;'>Si no fuiste vos quien solicitó el cambio, podés ignorar este
                mensaje. Tu cuenta sigue segura.</p>

            <!-- Equipo firmado -->
            <p style='margin-top: 40px; font-weight: bold; font-size: 15px; color: #9445b7;'>Equipo de Alquimia</p>
        </div>

        <!-- Footer -->
        <div style='background-color: #f4f4f4; text-align: center; padding: 20px; font-size: 12px; color: #999;'>
            © 2025 Alquimia — Todos los derechos reservados
            <br /><br />
            <a href='https://instagram.com/alquimia.frag' target='_blank'
                style='text-decoration: none; display: inline-flex; align-items: center; gap: 6px; color: #9445b7; font-weight: bold; font-size: 13px;'>
                <img src='http://upload.wikimedia.org/wikipedia/commons/a/a5/Instagram_icon.png' width='16' height='16'
                    alt='Instagram' style='display: inline-block;' />
                @alquimia.frag
            </a>
        </div>
    </div>";
        }

        public string GetWelcomeEmail(string userName)
        {
            return $@"
                <div
        style=""font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; border: 1px solid #ddd; border-radius: 8px; overflow: hidden;"">

        <!-- 🖼️ Imagotipo -->
        <div style=""text-align: center; padding: 20px; background-color: #fff;"">
            <img src=""https://res.cloudinary.com/dxnwcmh1j/image/upload/v1750382478/imagotipo_mpkd4p.png"" alt=""Alquimia Logo""
                width=""120"" style=""margin: auto;"" />
        </div>

        <!-- 📄 Cuerpo -->
        <div style=""padding: 30px; background-color: #fff;"">
            <p style=""font-size: 16px;"">Hola, <strong>{userName}</strong>.</p>

            <p style=""font-size: 15px;"">¡Gracias por registrarte como proveedor en Alquimia!</p>

            <p style=""font-size: 15px;"">Tu cuenta ha sido creada exitosamente, pero antes de que puedas comenzar a cargar tus productos, nuestro equipo debe aprobarla.</p>

            <p style=""font-size: 15px;"">Te enviaremos un correo de confirmación cuando tu cuenta esté habilitada.</p>

            <p style=""font-size: 14px; color: #555;"">Agradecemos tu paciencia y tu interés en formar parte de nuestra comunidad.</p>

            <!-- Firma -->
            <p style=""margin-top: 40px; font-weight: bold; font-size: 15px; color: #9445b7;"">Equipo de Alquimia</p>
        </div>

        <!-- 📎 Footer -->
        <div style=""background-color: #f4f4f4; text-align: center; padding: 20px; font-size: 12px; color: #999;"">
            © 2025 Alquimia — Todos los derechos reservados
            <br /><br />
            <a href=""https://instagram.com/alquimia.frag"" target=""_blank""
                style=""text-decoration: none; display: inline-flex; align-items: center; gap: 6px; color: #9445b7; font-weight: bold; font-size: 13px;"">
                <img src=""http://upload.wikimedia.org/wikipedia/commons/a/a5/Instagram_icon.png"" width=""16"" height=""16""
                    alt=""Instagram"" style=""display: inline-block;"" />
                @alquimia.frag
            </a>
        </div>
    </div>";
        }

    }
}
