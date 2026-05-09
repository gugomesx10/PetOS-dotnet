using Microsoft.AspNetCore.Mvc;
using PetOS.Services;
using PetOS.Services.Interfaces;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertController : ControllerBase
{
    private readonly IAlertService _service;

    public AlertController(AlertService service)
    {
        _service = service;
    }
    
    

}