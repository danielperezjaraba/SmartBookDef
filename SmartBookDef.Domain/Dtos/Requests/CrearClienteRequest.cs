using SmartBookDef.Domain.DataAnotations;
using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.Dtos.Requests;
public record CrearClienteRequest(
    [SoloNumeros]
    string identificacion,
    string nombreCompleto,
    [CorreoCecar]
    string emailUsuario,
    [CelularDiezDigitos]
    string celular,
    DateOnly fechaNacimiento
    );
