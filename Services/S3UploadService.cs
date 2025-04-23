using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using Amazon.Runtime;

namespace EventFinderAPI.Services
{
    public class S3UploadService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "images-for-ubc-event-finder";
       

        public S3UploadService(IAmazonS3 s3Client)
        {
            if(s3Client == null) throw new ArgumentNullException(nameof(s3Client));
            _s3Client = s3Client;


        }

        public string GeneratePreSignedUploadUrl(string key)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(15),

                };

                return _s3Client.GetPreSignedURL(request);
            }
            catch (AmazonClientException ex)
            {
                Console.WriteLine("❌ AWS credentials not found or invalid.");
                Console.WriteLine(ex.Message);
                return "";
            }



        }
    }
}
