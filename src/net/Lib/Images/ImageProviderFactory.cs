using Lib.Images.Providers;

namespace Lib.Images
{
    public static class ImageProviderFactory
    {
        public static IImageProvider Get(string type)
        {
            switch (type)
            {
                case "RedditMemes":
                    return new RedditMemeProvider();
                case "Bing":
                    return new BingImageSearchProvider();
                default:
                    return new RedditMemeProvider();
            }
        }
    }
}