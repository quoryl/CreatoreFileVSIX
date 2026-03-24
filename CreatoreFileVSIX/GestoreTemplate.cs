namespace CreatoreFileVSIX
{
    public static class GestoreTemplate
    {
        public static string OttieniController(string nomeProgetto, string nomeEntita)
        {
            return $@"using System;
using Microsoft.AspNetCore.Mvc;

namespace {nomeProgetto}.FeatureControllers
{{
    [ApiController]
    [Route(""api/[controller]"")]
    public class {nomeEntita}Controller : ControllerBase
    {{
        [HttpGet]
        public IActionResult Get() => Ok(""Get {nomeEntita}"");
    }}
}}";
        }

        public static string OttieniInterfacciaServizio(string nomeProgetto, string nomeEntita)
        {
            return $@"using System;

namespace {nomeProgetto}
{{
    public interface I{nomeEntita}Service
    {{
    }}
}}";
        }

        public static string OttieniServizio(string nomeProgetto, string nomeEntita)
        {
            return $@"using System;

namespace {nomeProgetto}
{{
    public class {nomeEntita}Service : I{nomeEntita}Service
    {{
        public {nomeEntita}Service()
        {{
            // Costruttore
        }}
    }}
}}";
        }

        public static string OttieniInterfacciaProxy(string nomeProgetto, string nomeEntita)
        {
            return $@"using System;

namespace {nomeProgetto}
{{
    public interface I{nomeEntita}Proxy
    {{
        // TODO: Definisci qui i metodi proxy per {nomeEntita}
    }}
}}";
        }

        public static string OttieniProxy(string nomeProgetto, string nomeEntita)
        {
            return $@"using System;

namespace {nomeProgetto}
{{
    public class {nomeEntita}Proxy : I{nomeEntita}Proxy
    {{
        public {nomeEntita}Proxy()
        {{
            // Costruttore
        }}
    }}
}}";
        }

        public static string OttieniEntita(string nomeProgetto, string nomeEntita)
        {
            return $@"using System;

namespace {nomeProgetto}
{{
    public class {nomeEntita}Entity
    {{
        public int Id {{ get; set; }}
        
        // TODO: Aggiungi qui le altre colonne/proprietà della tabella {nomeEntita}
    }}
}}";
        }

    }
}