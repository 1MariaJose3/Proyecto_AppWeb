using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace PaginaMaestra.Clases
{
    public class CampusData
    {
        public Dictionary<string, Campus> campus { get; set; }
    }

    public class Campus
    {
        public string titulo { get; set; }
        public List<BotonEnlace> botonesFlotantes { get; set; }
        public List<string> logos { get; set; }
        public HorarioAtencion horarioAtencion { get; set; }
        public List<MenuItemm> menu { get; set; }
        public List<RedSocial> redes_sociales { get; set; }
        public string video { get; set; }
        public CampusInfo campusinfo { get; set; }
        public List<BotonEnlace> botonesComunidad { get; set; }
        public OfertaAcademica ofertaAcademica { get; set; }
        public List<BotonEnlace> botonesCirculares { get; set; }
        public Dictionary<string, BotonEnlace> campusOfertas { get; set; }
        public string siguenos { get; set; }
        public Footer footer { get; set; }
    }

    public class BotonEnlace
    {
        public string imagen { get; set; }
        public string enlace { get; set; }
    }

    public class MenuItemm
    {
        public string titulo { get; set; }
        public List<OpcionSimple> opciones { get; set; }
        public List<Categoria> categorias { get; set; }
        public string href { get; set; }
    }
    public class OpcionSimple
    {
        public string titulo { get; set; }
        public string enlace { get; set; }
        public string claveVista { get; set; }
        public ContenidoOpcionSimple contenido { get; set; }
    }

    public class ContenidoOpcionSimple
    {
        public string imagenBanner { get; set; }
        public Mensaje mensajeRector { get; set; }
    }

    public class Mensaje
    {
        public string titulo { get; set; }
        public List<string> parrafos { get; set; }
        public string iframeMapa { get; set; }
        public string titulo2 { get; set; }
        public List<string> lista { get; set; }
        public List<string> parrafosFinales { get; set; }
        public List<Seccion> secciones { get; set; }
        public List<CategoriaSimple> categorias { get; set; }
    }

    public class Seccion
    {
        public string subtitulo { get; set; }
        public string subtitulos { get; set; }
        public List<string> parrafos { get; set; }
        public List<string> lista { get; set; }
        public List<string> parrafos1 { get; set; }
        public string imagenHospedaje { get; set; }
        public string imagenHospedaje2 { get; set; }
        public string imagenHospedaj3 { get; set; }
        public string enlace { get; set; }
        public List<string> nota { get; set; }
        public string subtitulo2 { get; set; }
        public List<string> parrafo1 { get; set; }
        public List<string> parrafo2 { get; set; }
        public List<string> imagenesRestaurantes { get; set; }
        public List<Seccion> secciones { get; set; }

    }

    public class CategoriaSimple
    {
        public string nombre { get; set; }
        public List<ProgramaSimple> programas { get; set; }
    }

    public class ProgramaSimple
    {
        public string nombre { get; set; }
        public string inicio { get; set; }
        public string duracion { get; set; }
        public string enlace { get; set; }
        public string ciudad { get; set; }
    }

    public class Categoria
    {
        public string nombre { get; set; }
        public List<Programa> programas { get; set; }
    }

    public class Programa
    {
        public string nombre { get; set; }
        public string enlace { get; set; }
        public string imagen { get; set; }
        public string tipo { get; set; }
        public ContenidoPrograma contenido { get; set; }
    }

    public class ContenidoPrograma
    {
        public string imagenBanner { get; set; }
        public InfoPrograma mensajeRector { get; set; }
    }

    public class InfoPrograma
    {
        public string titulo { get; set; }
        public string subtitulo { get; set; }
        public string inicio { get; set; }
        public string duracion { get; set; }
        public string modalidad { get; set; }
        public string horarios { get; set; }
        public string dirigidoA { get; set; }

        [JsonConverter(typeof(UniversidadConverter))]
        public Universidad universidad { get; set; }
        public ObjetivoGeneral objetivoGeneral { get; set; }
        public ObjetivoEspecifico objetivoEspecifico { get; set; }
        public ValorCurricular valorCurricular { get; set; }
        public PlanEstudios planEstudios { get; set; }
        public PerfilEgreso perfilEgreso { get; set; }
        public CompetenciasProfesionales competenciasProfesionales { get; set; }
        public Titulacion titulacion { get; set; }
        public ProgramaAcademico programaAcademico { get; set; }
        public Rvoe rvoe { get; set; }
    }

    public class Universidad
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }
    }

    public class ObjetivoGeneral
    {
        public string subtitulo { get; set; }
        public string descripcion { get; set; }
    }

    public class ObjetivoEspecifico
    {
        public string subtitulo { get; set; }
        public List<string> lista { get; set; }
    }

    public class PlanEstudios
    {
        public string subtitulo { get; set; }
        public string diploma { get; set; }
    }

    public class PerfilEgreso
    {
        public string subtitulo { get; set; }
        public string descripcion { get; set; }
    }

    public class ValorCurricular
    {
        public string subtitulo { get; set; }
        public string descripcion { get; set; }
        public string subtitulo2 { get; set; }
        public string empresa { get; set; }
    }

    public class CompetenciasProfesionales
    {
        public string tituloSeccion { get; set; }

        [JsonConverter(typeof(ConocimientoConverter))]
        public List<Conocimiento> conocimientos { get; set; }

        [JsonConverter(typeof(ConocimientoConverter))]
        public List<Conocimiento> habilidades { get; set; }

        [JsonConverter(typeof(ConocimientoConverter))]
        public List<Conocimiento> actitudes { get; set; }

        [JsonConverter(typeof(ConocimientoConverter))]
        public List<Conocimiento> valores { get; set; }
    }


    public class Conocimiento
    {
        public string texto { get; set; } 
        public List<string> sublista { get; set; } 
    }

    public class Titulacion
    {
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string subtituloOpciones { get; set; }
        public List<string> opciones { get; set; }
        public string subtituloEntrega { get; set; }
        public List<string> alFinalizar { get; set; }
    }

    public class ProgramaAcademico
    {
        public string tituloSeccion { get; set; }
        public List<List<string>> semestres { get; set; }
        public List<List<Modulo>> modulos { get; set; }
    }

    public class Modulo
    {
        public string titulo { get; set; }
        public string texto { get; set; }
        public List<string> sublista { get; set; }
        public string nota { get; set; }
    }

    public class Rvoe
    {
        public string titulo { get; set; }
        public string clave { get; set; }
    }


    public class HorarioAtencion
    {
        public string lunesViernes { get; set; }
        public string sabado { get; set; }
        public string informes { get; set; }
        public List<string> soporte { get; set; }
        public string diplomados { get; set; }
    }

    public class Footer
    {
        public string imagen { get; set; }
        public string derechos { get; set; }
        public List<Informacion> informacion { get; set; }
    }

    public class Informacion
    {
        public string titulo { get; set; }
        public List<string> texto { get; set; }
        public List<string> telefonos { get; set; }
    }

    public class OfertaAcademica
    {
        public string tituloSeccion { get; set; }
        public List<ItemOferta> items { get; set; }
    }

    public class ItemOferta
    {
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string imagen { get; set; }
        public string enlace { get; set; }
    }

    public class CampusInfo
    {
        public string titulo { get; set; }
        public string imagenPrincipal { get; set; }
        public List<string> descripcion { get; set; }
        public Invitacion invitacion { get; set; }
        public string imagenFrase { get; set; }
    }

    public class Invitacion
    {
        public string texto { get; set; }
        public string iconoIzquierdo { get; set; }
        public string iconoDerecho { get; set; }
    }

    public class RedSocial
    {
        public string nombre { get; set; }
        public string url { get; set; }
    }


    public class ConocimientoConverter : JsonConverter<List<Conocimiento>>
    {
        public override List<Conocimiento> ReadJson(JsonReader reader, Type objectType, List<Conocimiento> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = new List<Conocimiento>();

            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
            {
                foreach (var item in token)
                {
                    if (item.Type == JTokenType.String)
                    {
                        result.Add(new Conocimiento { texto = item.ToString() });
                    }
                    else if (item.Type == JTokenType.Object)
                    {
                        result.Add(item.ToObject<Conocimiento>());
                    }
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, List<Conocimiento> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var conocimiento in value)
            {
                if (conocimiento.sublista == null || conocimiento.sublista.Count == 0)
                {
                    // Solo texto, lo serializamos como string simple
                    writer.WriteValue(conocimiento.texto);
                }
                else
                {
                    // Tiene sublista, serializamos como objeto con propiedades
                    writer.WriteStartObject();

                    writer.WritePropertyName("texto");
                    writer.WriteValue(conocimiento.texto);

                    writer.WritePropertyName("sublista");
                    serializer.Serialize(writer, conocimiento.sublista);

                    writer.WriteEndObject();
                }
            }

            writer.WriteEndArray();
        }
    }

    public class UniversidadConverter : JsonConverter<Universidad>
    {
        public override Universidad ReadJson(JsonReader reader, Type objectType, Universidad existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            var universidad = new Universidad
            {
                nombre = obj["nombre"]?.ToString()
            };

            JToken descripcionToken = obj["descripcion"];

            if (descripcionToken != null)
            {
                if (descripcionToken.Type == JTokenType.String)
                {
                    universidad.descripcion = descripcionToken.ToString();
                }
                else if (descripcionToken.Type == JTokenType.Array)
                {
                    // Unir los elementos del array en un solo string con doble salto de línea
                    universidad.descripcion = string.Join("\n\n", descripcionToken.Select(t => t.ToString()));
                }
            }

            return universidad;
        }

        public override void WriteJson(JsonWriter writer, Universidad value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("nombre");
            writer.WriteValue(value.nombre);

            writer.WritePropertyName("descripcion");

            // O puedes cambiar la lógica para escribir como string o array
            writer.WriteValue(value.descripcion);  // Escribe como string
            writer.WriteEndObject();
        }
    }


}