using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

public class BaseApiController : Controller
{
    protected readonly IServices Services;

    public BaseApiController(IServices services)
    {
        Services = services;
    }

    protected static ServiceResult Wrap(object data)
    {
        return new ServiceResult {Data = data}.BeSuccess();
    }

    protected static ServiceResult Wrap()
    {
        return new ServiceResult().BeSuccess();
    }
}