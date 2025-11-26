using SmartBookDef.Domain.DataAnotations;
using System.ComponentModel.DataAnnotations;

namespace SmartBookDef.Domain.Dtos.Requests;
public record ActualizarClienteRequest(

        string nombreCompleto,
        DateOnly fechaNacimiento,
        [CorreoCecar]
        string emailUsuario,
        [CelularDiezDigitos]
        string celular
        );
