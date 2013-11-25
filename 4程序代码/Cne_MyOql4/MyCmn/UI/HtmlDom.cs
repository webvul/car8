using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCmn
{
    public class HtmlNode : ICloneable
    {
        public enum NodeType
        {
            Tag,
            Text,
            Note,
            CloseTag,
        }
        public NodeType Type { get; set; }
        public int Level { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    public class HtmlTextNode : HtmlNode
    {
        public HtmlTextNode()
        {
            Type = NodeType.Text;
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class HtmlNoteNode : HtmlNode
    {
        public HtmlNoteNode()
        {
            Type = NodeType.Note;
        }
        public string Text { get; set; }
        public override string ToString()
        {
            return Text;
        }
    }

    public class HtmlAttrNode
    {
        public string Name { get; set; }
        public bool IsSole { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            if (IsSole && Value.HasValue() == false)
            {
                return Name;
            }
            else return string.Format(@"{0}={1}", Name,
                Value.HasValue() && Value.Length > 0 && Value[0].ToString().IsIn("'", @"""") && Value[0] == Value[Value.Length - 1] ?
                Value : @"""" + Value + @""""
                );
        }
    }
    public class HtmlTagNode : HtmlNode
    {
        public HtmlTagNode()
        {
            Type = NodeType.Tag;
            IsSole = false;
            Attrs = new List<HtmlAttrNode>();
        }
        public bool IsSole { get; set; }
        public string TagName { get; set; }
        public List<HtmlAttrNode> Attrs { get; set; }

        public override string ToString()
        {
            string retVal = "";
            if (Attrs.Count == 0)
            {
                retVal = string.Format(@"<{0}", TagName);
            }
            else
            {
                retVal = string.Format(@"<{0} {1}", TagName,
                    string.Join(" ", Attrs.Select(o => o.ToString()).ToArray()));
            }

            if (IsSole)
            {
                retVal += " />";
            }
            else retVal += ">";

            return retVal;
        }
    }
    public class HtmlCloseTagNode : HtmlNode
    {
        public HtmlCloseTagNode()
        {
            Type = NodeType.CloseTag;
        }

        public string TagName { get; set; }

        public override string ToString()
        {
            return string.Format(@"</{0}>", TagName);
        }
    }

}
