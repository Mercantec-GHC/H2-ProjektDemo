using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;

namespace API.Service
{
    public class R2Service
    {
        private IAmazonS3 _s3Client;
        public PutObjectResponse UploadResponse { get; set; }
        public string accessKey { get; set; }
        public string secretKey { get; set; }

        public R2Service(string accessKey, string secretKey)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
        }
        public async Task<string> UploadToR2(Stream fileStream, string fileName)
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
            {
                ServiceURL = "https://7edebe7b5106f3bceb95ecd71d962f10.r2.cloudflarestorage.com",
            });

            var request = new PutObjectRequest
            {
                InputStream = fileStream,
                BucketName = "h2-hotel",
                Key = fileName,
                DisablePayloadSigning = true
            };

            UploadResponse = await _s3Client.PutObjectAsync(request);
            var imageUrl = $"https://hotel.magsapi.com/{request.Key}";
            return imageUrl;
        }
    }
}