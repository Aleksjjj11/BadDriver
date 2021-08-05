using System;
using System.IO;
using System.Threading.Tasks;
using Firebase.Storage;
using MobileApps.Interfaces;

namespace MobileApps.Models
{
    public static class FirebaseManager
    {
        private static string _storageUrl = "baddriver-37ea0.appspot.com";

        public static async Task<string> UploadImage(Stream fileResult, IUser user)
        {
            var storageImage = await new FirebaseStorage(_storageUrl)
                .Child($"{user.Username}")
                .Child($"{Guid.NewGuid()}.jpg")
                .PutAsync(fileResult);
            return storageImage;
        }
    }
}