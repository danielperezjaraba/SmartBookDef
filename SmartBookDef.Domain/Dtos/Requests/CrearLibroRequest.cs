using SmartBookDef.Domain.Enums;

namespace SmartBookDef.Domain.Dtos.Requests;
public record CrearLibroRequest(
    string Nombre,
    string Nivel,
    int Stock,
    int Tipo,
    string Editorial,
    string Edicion
);