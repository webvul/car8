using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace MyCmn
{
    /// <summary>
    /// 邮件发送辅助类。
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// 发送邮件。 [★]
        /// </summary>
        /// <param name="FromEmail"></param>
        /// <param name="Password"></param>
        /// <param name="SmtpServer"></param>
        /// <param name="ToEmails"></param>
        /// <param name="Title"></param>
        /// <param name="Body"></param>
        /// <param name="WithFiles"></param>
        /// <returns></returns>
        public static bool Send(string FromEmail, string Password, string SmtpServer, List<string> ToEmails, string Title, string Body, List<Attachment> WithFiles)
        {
            if (string.IsNullOrEmpty(FromEmail)) return false;
            if (string.IsNullOrEmpty(Password)) return false;
            if (ToEmails == null || ToEmails.Count == 0) return false;

            MailMessage mailMsg = new MailMessage();
            mailMsg.From = new MailAddress(FromEmail);

            foreach (string item in ToEmails)
            {
                mailMsg.To.Add(item);
            }
            mailMsg.Subject = Title;
            mailMsg.Body = Body;
            mailMsg.BodyEncoding = Encoding.UTF8;
            mailMsg.Priority = MailPriority.High;

            if (WithFiles != null)
            {
                foreach (Attachment item in WithFiles)
                {
                    mailMsg.Attachments.Add(item);
                }
            }

            SmtpClient smtp = new SmtpClient();
            // 提供身份验证的用户名和密码
            // 网易邮件用户可能为：username password
            // Gmail 用户可能为：username@gmail.com password
            smtp.Credentials = new NetworkCredential(FromEmail.Substring(0, FromEmail.IndexOf("@")), Password);
            smtp.Port = 25; // Gmail 使用 465 和 587 端口
            smtp.Host = SmtpServer; // 如 smtp.163.com, smtp.gmail.com
            smtp.EnableSsl = false; // 如果使用GMail，则需要设置为true
            smtp.Send(mailMsg);
            return true;
        }

        /// <summary>
        /// 发送邮件 [★]
        /// </summary>
        /// <param name="ToEmails"></param>
        /// <param name="Title"></param>
        /// <param name="Body"></param>
        /// <param name="WithFiles"></param>
        /// <returns></returns>
        public static bool Send(List<string> ToEmails, string Title, string Body, List<Attachment> WithFiles)
        {
            if (ToEmails == null || ToEmails.Count == 0) return false;

            MailMessage mailMsg = new MailMessage();

            foreach (string item in ToEmails)
            {
                mailMsg.To.Add(item);
            }
            mailMsg.Subject = Title;
            mailMsg.Body = Body;
            mailMsg.BodyEncoding = Encoding.UTF8;
            mailMsg.Priority = MailPriority.High;

            if (WithFiles != null)
            {
                foreach (Attachment item in WithFiles)
                {
                    mailMsg.Attachments.Add(item);
                }
            }

            SmtpClient smtp = new SmtpClient();
            // 提供身份验证的用户名和密码
            // 网易邮件用户可能为：username password
            // Gmail 用户可能为：username@gmail.com password
            smtp.EnableSsl = false; // 如果使用GMail，则需要设置为true
            smtp.Send(mailMsg);
            return true;
        }

    }
}
