using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SilkDirectX11.Converters
{
    public class ConvertEnum : MarkupExtension
    {
        public Type EnumType { get; set; }

        public ConvertEnum() { }

        public ConvertEnum(Type enumType)
        {
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType == null || !EnumType.IsEnum)
                return null;

            return Enum.GetValues(EnumType);
        }
    }
}
