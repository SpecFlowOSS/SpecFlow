using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers {

    public class BoolValueRetriever : StructRetriever<bool> {
        private static readonly List<string> MultiLingualTrueValues = new List<string> {
            "true", "1", "thật", "to'g'ri", "to'g'ri", "سچ ہے", "правда", "doğru", "จริง", "నిజం", "дөрес", "உண்மை",
            "sann", "verdadera", "verdadero", "prav", "pravda", "istina", "истинито", "истина", "adevărat", "verdade",
            "prawdziwe", "درست است، واقعی", "ਸੱਚ ਹੈ", "hold", "ekte", "үнэн", "benar", "richteg", "motina", "taisnība",
            "진실", "teH", "ನಿಜ", "bener", "vera", "vero", "satt", "igaz", "सच", "נָכוֹן", "સાચું", "αληθής", "stimmt", "certo",
            "vraise", "totta", "tõsi", "vera", "waark", "Sandt", "vre", "真", "真", "veritable", "истински", "istinito", "ճիշտ", "صحيح", "wier"
        };

        protected override bool GetNonEmptyValue(string value) {
            return !string.IsNullOrWhiteSpace(value) && MultiLingualTrueValues.Contains(value.ToLower());
        }
    }
}