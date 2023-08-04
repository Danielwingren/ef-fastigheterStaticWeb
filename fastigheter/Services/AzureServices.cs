using Azure;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using Azure.Storage.Blobs;
using fastigheter.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace fastigheter.Services
{
    public class AzureServices
    {

        public static void CreateJson (Intresse intresse)
        {
            var json = JsonConvert.SerializeObject(intresse);
            Console.WriteLine(json);
            List<string> onlyNames = new();
            var splitNames = intresse.NameAndSocialSecurity?.Split(' ');
            if (splitNames != null)
                foreach (var s in splitNames)
                {
                    if (!Regex.IsMatch(s, @"\d"))
                    {
                        onlyNames.Add(s);
                    }
                }

            var firstNameOfApllication = onlyNames[0] + "." + onlyNames[1];
            if (intresse.NameAndSocialSecurity != null) UploadJson(json, firstNameOfApllication, intresse.Id);
        }
        public static async void UploadJson(string json, string nameOfApplicant, string id) 
        {
            var blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=edvardssonstorage;AccountKey=0jlAIbijk9fWgU2JgPJdd7ChyJXzLUV22gQWwtkFCZLlDcQJive04wsObRxTzGCCio9b3O7l1CuJ+ASt+n+YqA==;EndpointSuffix=core.windows.net";
           
            var blobStorageContainerName = "intresse";
            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainerName);

            BlobClient blob = container.GetBlobClient(nameOfApplicant+id+".json");

            using MemoryStream ms = new(Encoding.UTF8.GetBytes(json));
            try
            {
                await blob.UploadAsync(ms);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void CreateJsonErrors(ErrorReport errorReport)
        {
            var json = JsonConvert.SerializeObject(errorReport);
            Console.WriteLine(json);
            var nameOfFile = errorReport.Name + "_" +errorReport.Id;

            if (nameOfFile != null) UploadJsonErrors(json, nameOfFile);
        }
        public static async void UploadJsonErrors(string json, string nameOfFile)
        {
            var blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=edvardssonstorage;AccountKey=0jlAIbijk9fWgU2JgPJdd7ChyJXzLUV22gQWwtkFCZLlDcQJive04wsObRxTzGCCio9b3O7l1CuJ+ASt+n+YqA==;EndpointSuffix=core.windows.net";

            var blobStorageContainerName = "errors";
            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainerName);

            BlobClient blob = container.GetBlobClient(nameOfFile + ".json");

            using MemoryStream ms = new(Encoding.UTF8.GetBytes(json));
            try
            {
                await blob.UploadAsync(ms);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void SendEmailIntresse (Intresse intresse)
        {
            string orterAvIntresse = "";
            if (intresse.Skärstad) { orterAvIntresse += "Skärstad\n"; }
            if (intresse.Ölmstad) { orterAvIntresse += "Ölmstad"; }
            string connectionString = "endpoint=https://edvardssonfastigheter.communication.azure.com/;accesskey=KLGVv1H7X6LxVPqNhRBVhkF8L9PKhVTVRYIG0JuxuAEWcch5vsukU/FYhpQvWryuPGVwhR+hlNfbOkWVO93L4w==";
            EmailClient emailClient = new(connectionString);
            string paymentNoteInfo = "";
            if (intresse.PaymentNote != null)
            {
                paymentNoteInfo = PaymentInformation((bool)intresse.PaymentNote);
            }
           
            EmailContent emailContentMikael = new("Inkommande Intresseanmälan: " + intresse.Id)
            {
                PlainText = "Ny intresseanmälan för lägenhet har publicerats på Azure:  \n" +
                "Information om sökande: \n" + intresse.NameAndSocialSecurity + "\n" +
                "Email: " + intresse.PrimaryEmail + 
                "\nMobil: " + intresse.PrimaryPhone + "\n" +
                "\nSysselsättning: " + intresse.Occupation +
                "\nÖnskat inflyttningsdatum: " + intresse.DateToMove +
                "Betalningsanmärkningar: "+paymentNoteInfo +
                "Referens: "+intresse.Reference + "\n\n" +
                "Ort(er) av intresse: " + "\n" +
                 orterAvIntresse +
                "\nMeddelande: \n" + intresse.SizeAndComments + "\n" +
                "\nMVH\n(numera) Webutvecklare Danne!"
            };
            List<EmailAddress> emailAddresses = new()
            {
                new EmailAddress("danielwingren@live.se") { DisplayName = "Nyinkommen Intresseanmälan!" },
                new EmailAddress("mikael@edvardsson.se") { DisplayName = "Nyinkommen Intresseanmälan!" }
            };

            EmailContent emailContenSökandel = new("Tack för din ansökan! " + intresse.Id)
            {
                PlainText = "Hej!\nKul att du är intresserad av att hyra en av våra lägenheter!\n\n" +
                            "Vi har nu tagit emot din ansökan och vi kommer nu att gå igenom den för att försöka hitta ditt nya drömboende."+
                            "\nJust nu har vi tyvärr inga tillgängliga lägenheter, men när en lägenhet blir tillgänglig kommer vi att kolla mot din ansökan om det kan vara något som passar just dig!"+
                            "\nVid frågor är du välkommen att kontakta vår boende-support på: 0760162267"+
                            "\nMVH\nEdvardssons Fastigheter!"
            };
            List<EmailAddress> emailAddressSökande = new()
            {
                new EmailAddress(intresse.PrimaryEmail) { DisplayName = "Edvardssons Fastigheter" }
            };

            //Mejl till EF
            EmailRecipients emailRecipients = new(emailAddresses);
            EmailMessage emailMessage = new("DoNotReply@c9ab821a-f2af-451c-b2ce-915c37638039.azurecomm.net", emailContentMikael, emailRecipients);
            SendEmailResult emailResult = emailClient.Send(emailMessage, CancellationToken.None);
            //Mejl till Sökande
            EmailRecipients emailRecipientSökande = new(emailAddressSökande);
            EmailMessage emailMessage2 = new("DoNotReply@c9ab821a-f2af-451c-b2ce-915c37638039.azurecomm.net", emailContenSökandel, emailRecipientSökande);
            SendEmailResult emailResult2 = emailClient.Send(emailMessage2, CancellationToken.None);
            //Console.WriteLine($"This is the messageId of the newly created email: {emailResult.MessageId}");
            //MessageStatus(emailClient, emailResult);
            //MessageStatus(emailClient, emailResult2);
        }

        public static void SendEmailIssueReport(ErrorReport errorReport)
        {

            string connectionString = "endpoint=https://edvardssonfastigheter.communication.azure.com/;accesskey=KLGVv1H7X6LxVPqNhRBVhkF8L9PKhVTVRYIG0JuxuAEWcch5vsukU/FYhpQvWryuPGVwhR+hlNfbOkWVO93L4w==";
            EmailClient emailClient = new(connectionString);
            string enterWithKey = "NEJ";
#pragma warning disable CS8629 // Nullable value type may be null.
            try
            {
                if ((bool)errorReport.AccessWithKeyYes)
                {
                    enterWithKey = "JA";
                }
            } 
            catch (Exception)
            {
                if ((bool)errorReport.AccessWithKeyNo)
                {
                    enterWithKey = "NEJ";
                }
            }
            

#pragma warning restore CS8629 // Nullable value type may be null.

            EmailContent emailContentMikael = new("Felanmälan: lgh" + errorReport.AppartmentNumber + "Id:"+errorReport.Id)
            {
                PlainText = "Ny felanmälan för lägenhet" + errorReport.AppartmentNumber +" har publicerats på Azure:  \n" +
                "Namn på boende: \n" + errorReport.Name+ "\n" +
                "Email: " + errorReport.Mail +
                "\nMobil: " + errorReport.Phone + "\n" +
                "\nOk att gå in med nyckel?\t " + enterWithKey +
                "\n\nFel beskrivning: " + errorReport.ErrorDescription+
                "\n\nId: " + errorReport.Id + "\n" +
                "Inskickat: " + errorReport.TimeOfSubmit + "\n" +
                "\nMVH\n(numera) Webutvecklare Danne!"
            };
            List<EmailAddress> emailAddresses = new()
            {
                new EmailAddress("danielwingren@live.se") { DisplayName = "Nyinkommen felanmälan!" },
                new EmailAddress("mikael@edvardsson.se") { DisplayName = "Nyinkommen felanmälan!" }
            };

            EmailContent emailContenSökandel = new("Felanmälan mottagen! (Id: " + errorReport.Id+")")
            {
                PlainText = "Hej!\n" +
                "Vi har mottagit din felanmälan och kommer åtgärda detta snarast. Vid frågor kommer vi att kontakta dig på det telefonnummer du uppgav vid inskickandet av denna felanmälan."+
                "\nOm du har några frågor är du välkommen att kontakta vår boende-support på: 0760162267" +
                "\nMVH\nEdvardssons Fastigheter!"
            };
            List<EmailAddress> emailAddressSökande = new()
            {
                new EmailAddress(errorReport.Mail) { DisplayName = "Edvardssons Fastigheter" }
            };

            //Mejl till EF
            EmailRecipients emailRecipients = new(emailAddresses);
            EmailMessage emailMessage = new("DoNotReply@c9ab821a-f2af-451c-b2ce-915c37638039.azurecomm.net", emailContentMikael, emailRecipients);
            SendEmailResult emailResult = emailClient.Send(emailMessage, CancellationToken.None);
            //Mejl till Sökande
            EmailRecipients emailRecipientSökande = new(emailAddressSökande);
            EmailMessage emailMessage2 = new("DoNotReply@c9ab821a-f2af-451c-b2ce-915c37638039.azurecomm.net", emailContenSökandel, emailRecipientSökande);
            SendEmailResult emailResult2 = emailClient.Send(emailMessage2, CancellationToken.None);
            //Console.WriteLine($"This is the messageId of the newly created email: {emailResult.MessageId}");
            //MessageStatus(emailClient, emailResult);
            //MessageStatus(emailClient, emailResult2);
        }
        public static void MessageStatus (EmailClient emailClient,SendEmailResult emailResult)
        {
            Response<SendStatusResult>? messageStatus = emailClient.GetSendStatus(emailResult.MessageId);
            Console.WriteLine($"MessageStatus = {messageStatus.Value.Status}");
            TimeSpan duration = TimeSpan.FromMinutes(3);
            long start = DateTime.Now.Ticks;
            do
            {
                messageStatus = emailClient.GetSendStatus(emailResult.MessageId);
                if (messageStatus.Value.Status != SendStatus.Queued)
                {
                    Console.WriteLine($"MessageStatus = {messageStatus.Value.Status}");
                    break;
                }
                Thread.Sleep(10000);
                Console.WriteLine($"...");

            } while (DateTime.Now.Ticks - start < duration.Ticks);
        }

        private static string PaymentInformation(bool paymentNote)
        {
            if (paymentNote)
            {
                return "Personen har uppgett att hen har betalningsanmärkningar.";
            }

            return "Inga betalningsanmärkningar har uppgetts.";
        }
    }
}
