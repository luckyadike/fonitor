// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Fonitor.SimpleClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using Windows.Web.Http;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaCapture captureManager;
        DeviceInformationCollection devices;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            captureManager = new MediaCapture();

            if (devices.Count > 0)
            {
                await captureManager.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    VideoDeviceId = devices.ElementAt(1).Id,
                    PhotoCaptureSource = PhotoCaptureSource.VideoPreview
                });

                SetResolution();
            }
        }

        private async void SetResolution()
        {
            IReadOnlyList<IMediaEncodingProperties> resolution;
            resolution = captureManager.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview);

            uint maxResolution = 0;
            int indexMaxResolution = 0;

            if (resolution.Count >= 1)
            {
                for (int i = 0; i < resolution.Count; i++)
                {
                    var props = resolution[i] as VideoEncodingProperties;

                    if (props.Width > maxResolution)
                    {
                        indexMaxResolution = i;
                        maxResolution = props.Width;
                    }
                }

                await captureManager.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, resolution[indexMaxResolution]);
            }
        }

        private async void startSensorClick_Click(object sender, RoutedEventArgs e)
        {
            var encodingProperties = ImageEncodingProperties.CreateJpeg();

            using (var imageStream = new InMemoryRandomAccessStream())
            {
                await captureManager.CapturePhotoToStreamAsync(encodingProperties, imageStream);

                await UploadAsync(imageStream);
            }
        }

        private async Task UploadAsync(IRandomAccessStream stream)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-ApiKey", apiKey.Text.Trim());

                client.DefaultRequestHeaders.Add("X-SensorId", sensorId.Text.Trim());

                var endpoint = new Uri("https://fonitor.azurewebsites.net/api/image/upload");

                if (stream != null)
                {
                    stream.Seek(0);

                    var content = new HttpMultipartFormDataContent();

                    content.Add(new HttpStreamContent(stream));

                    var response = await client.PostAsync(endpoint, content);

                    apiKey.Text = response.ReasonPhrase;
                    sensorId.Text = response.Source.ToString();
                }

            }
        }
    }
}
