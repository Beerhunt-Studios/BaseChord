using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BaseChord.Api.Controller;

public abstract class BaseController : ControllerBase
{
    protected ISender Sender { get; init; }

    public BaseController(ISender sender) => Sender = sender;
}
