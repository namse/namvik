using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace namvik
{
    public enum Contents
    {
        ByeByeEffect,
    }

    public static class ContentLoader
    {
        private static readonly Dictionary<Contents, (string, Type)> ContentDescriptions = new Dictionary<Contents, (string, Type)>
        {
            { Contents.ByeByeEffect, ("shader/byeByeEffect", typeof(Effect)) },
        };
        private static readonly Dictionary<Contents,  object> ContentMap = new Dictionary<Contents, object>();

        public static void LoadContents(ContentManager contentManager)
        {
            foreach (var keyValuePair in ContentDescriptions)
            {
                var (assetName, type) = keyValuePair.Value;

                var method = typeof(ContentManager).GetMethod("Load");

                method = method.MakeGenericMethod(type);

                var content = method.Invoke(contentManager, new object[]{ assetName });

                ContentMap[keyValuePair.Key] = content;
            }
        }

        public static T GetContent<T>(Contents content) where T: class
        {
            return ContentMap[content] as T;
        }
    }
}
