
using System;
using System.IO;
using System.Text;


namespace Difftaculous.ZModel
{
    internal static class WriteExtensions
    {


        public static string AsJson(this ZToken token)
        {
            StringBuilder builder = new StringBuilder();
            using (StringWriter writer = new StringWriter(builder))
            {
                token.WriteJson(writer);
            }
            return builder.ToString();
        }



        public static void WriteJson(this ZToken token, TextWriter writer)
        {
            WriteTokenJson(token, writer);
        }



        private static void WriteTokenJson(ZToken token, TextWriter writer)
        {
            if (token is ZObject)
            {
                WriteJsonObject((ZObject)token, writer);
            }
            else if (token is ZValue)
            {
                WriteJsonValue((ZValue)token, writer);
            }
            else if (token is ZArray)
            {
                WriteJsonArray((ZArray)token, writer);
            }
            else
            {
                throw new NotImplementedException("Don't know how to write the JSON for: " + token.GetType().Name);
            }
        }


        private static void WriteJsonObject(ZObject obj, TextWriter writer)
        {
            writer.Write("{");

            bool first = true;
            foreach (var pair in obj)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(",");
                }

                writer.Write(" {0}: ", pair.Key);
                WriteTokenJson(pair.Value, writer);
            }

            writer.Write(" }");
        }



        private static void WriteJsonValue(ZValue val, TextWriter writer)
        {
            // TODO - what about multiple values?
            writer.Write(val.Value);
        }


        private static void WriteJsonArray(ZArray array, TextWriter writer)
        {
            writer.Write("[ ");

            bool first = true;
            foreach (var token in array)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(", ");
                }

                WriteTokenJson(token, writer);
            }

            writer.Write(" ]");
        }
    }
}
