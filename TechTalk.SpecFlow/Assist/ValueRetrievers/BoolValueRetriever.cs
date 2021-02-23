using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gherkin;
using Gherkin.Ast;
using Gherkin.Stream;
using TechTalk.SpecFlow.Configuration.AppConfig;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers {

    public class BoolValueRetriever : StructRetriever<bool> {
        protected override bool GetNonEmptyValue(string value) {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var MultiLingualTrueValues = new List<string> {
                "true", "1", "thật", "to'g'ri", "to'g'ri", "سچ ہے", "правда", "doğru", "จริง", "నిజం", "дөрес", "உண்மை",
                "sann", "verdadera", "verdadero", "prav", "pravda", "istina", "истинито", "истина", "adevărat", "verdade",
                "prawdziwe", "درست است، واقعی", "ਸੱਚ ਹੈ", "hold", "ekte", "үнэн", "benar", "richteg", "motina", "taisnība",
                "진실", "teH", "ನಿಜ", "bener", "vera", "vero", "satt", "igaz", "सच", "נָכוֹן", "સાચું", "αληθής", "stimmt", "certo",
                "vraise", "totta", "tõsi", "vera", "waark", "Sandt", "vre", "真", "真", "veritable", "истински", "istinito", "ճիշտ", "صحيح", "wier"
            };
            return MultiLingualTrueValues.Contains(value.ToLower());
        }
    }
}