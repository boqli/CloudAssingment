using Common;
using DataAccess.Interfaces;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class UsersController : Controller
    {
        private IFireStoreDataAccess fireStore;
        private IPubsubRepository pubsub;
        public UsersController(IFireStoreDataAccess _fireStore, IPubsubRepository _pubsub)
        {
            pubsub = _pubsub;
            fireStore = _fireStore;
        }

        [Authorize]
        public async Task< IActionResult> Index()
        {
            var myUser = await fireStore.GetUser(User.Claims.ElementAt(4).Value);
            if(myUser == null)
            {
                myUser = new Common.User();
                myUser.Email = User.Claims.ElementAt(4).Value;
            }
            return View(myUser);
        }

        public IActionResult Register(User user)
        {
            
            user.Email = User.Claims.ElementAt(4).Value;
            user.FirstName = User.Claims.ElementAt(2).Value;
            user.LastName = User.Claims.ElementAt(3).Value;
            
            fireStore.AddUser(user);
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> List()
        {
            List<User> users = await fireStore.GetAllUsers();
            return View(users);
        }


        [Authorize]
        public async Task<IActionResult> ListFiles()
        {
            var messages = await fireStore.GetFiles(User.Claims.ElementAt(4).Value);
            return View(messages);
        }

        [Authorize]
        public IActionResult Convert()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Send()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Send(Common.File msg, IFormFile file)
        {
            string id = Guid.NewGuid().ToString();
            if (file != null)
            {
                string bucketName = "boqlicloudprog";
                string fileName = file.FileName;
                string finalName = fileName + System.IO.Path.GetExtension(file.FileName);
                //upload the file on cloud storage
                var storage = StorageClient.Create();
                //            using var fileStream = System.IO.File.OpenRead(localPath);

                using (Stream fsIn = file.OpenReadStream())
                {
                    storage.UploadObject(bucketName, finalName, null, fsIn);
                }


                var storageObject = storage.GetObject(bucketName, finalName, new GetObjectOptions
                {
                    Projection = Projection.Full
                });

                storageObject.Acl.Add(new ObjectAccessControl
                {
                    Bucket = bucketName,
                    Entity = $"user-{User.Claims.ElementAt(4).Value}",
                    Role = "OWNER",
                });

                storageObject.Acl.Add(new ObjectAccessControl
                {
                    Bucket = bucketName,
                    Entity = $"user-{User.Claims.ElementAt(4).Value}",
                    Role = "READER",
                });

                var updatedObject = storage.UpdateObject(storageObject);
                
                msg.Attachment = $" https://storage.cloud.google.com/{bucketName}/{finalName}";
                msg.newLink = msg.Attachment.Substring(0, msg.Attachment.Length - 9) + "pdf";
                msg.Name = fileName;
                var time = DateTime.Now;
                var date = time.Date;
                var time2 = DateTime.SpecifyKind(date, DateTimeKind.Utc);
                msg.DateSent = time2;
                //file path is saved with the rest of the details
                //standard url to a public object: https://storage.googleapis.com/swd63b2022ra/3-Other%20Storage%20Options.pptx
                //https://storage.cloud.google.com/swd63b2022ra/61da9259-df23-4359-b3cc-e99a4d9cf622.jpg
                //msg.Attachment = $" https://storage.cloud.google.com/{bucketName}/{fileName}";
            }
            msg.Id = id; //if we were going to allow for more than 1 attachment, ids should be unique 
            await fireStore.SendMessage(User.Claims.ElementAt(4).Value, msg);
            await pubsub.Publish(msg);
            fireStore.minusCredit(User.Claims.ElementAt(4).Value);
            return RedirectToAction("ListFiles");
        }

        /*
        [HttpPost]
        [Authorize]
        public async Task< IActionResult> Send(Message msg, IFormFile file)
        { 
            string id = Guid.NewGuid().ToString();
            if (file != null)
            {
              
                string bucketName = "swd63b2022ra";
                string fileName = id + System.IO.Path.GetExtension(file.FileName);
                //upload the file on cloud storage
                var storage = StorageClient.Create();
                //            using var fileStream = System.IO.File.OpenRead(localPath);

                using (Stream fsIn = file.OpenReadStream())
                {
                    storage.UploadObject(bucketName, fileName, null, fsIn);
                }


                var storageObject = storage.GetObject(bucketName, fileName, new GetObjectOptions
                {
                    Projection = Projection.Full
                });

                storageObject.Acl.Add(new ObjectAccessControl
                {
                    Bucket = bucketName,
                    Entity = $"user-{User.Claims.ElementAt(4).Value}",
                    Role = "OWNER",
                });

                storageObject.Acl.Add(new ObjectAccessControl
                {
                    Bucket = bucketName,
                    Entity = $"user-{msg.Recipient}",
                    Role = "READER",
                });


                var updatedObject = storage.UpdateObject(storageObject);

                msg.Attachment = $" https://storage.cloud.google.com/{bucketName}/{fileName}";

                //file path is saved with the rest of the details
                //standard url to a public object: https://storage.googleapis.com/swd63b2022ra/3-Other%20Storage%20Options.pptx
                //https://storage.cloud.google.com/swd63b2022ra/61da9259-df23-4359-b3cc-e99a4d9cf622.jpg
                msg.Attachment = $" https://storage.cloud.google.com/{bucketName}/{fileName}";
            }
            msg.Id = id; //if we were going to allow for more than 1 attachment, ids should be unique 
           await fireStore.SendMessage(User.Claims.ElementAt(4).Value, msg);

            await pubsub.Publish(msg); 
            //1. queue the message in the topic 
            //2. it will trigger a cloud function which will connect with an email api and sends msg

            return RedirectToAction("List");
        }
       */



    }
}
