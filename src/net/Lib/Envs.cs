using System;
using DotNetEnv;
using System.IO;

namespace Lib
{
    public static class Envs
    {
        public static void Load()
        {
            string dotEnvFileNameEnv = Environment.GetEnvironmentVariable("DOTENV_FILENAME");
            string dotEnvFileName = !String.IsNullOrEmpty(dotEnvFileNameEnv) ? dotEnvFileNameEnv : DotNetEnv.Env.DEFAULT_ENVFILENAME;

            var pathParts = Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar);
            for (int i = pathParts.Length; i > 0; i--)
            {
                var path = Path.Combine(String.Join(Path.DirectorySeparatorChar.ToString(), pathParts, 0, i), dotEnvFileName);
                if (File.Exists(path))
                {
                    Env.Load(path);
                    break;
                }
            }
        }
    }
}