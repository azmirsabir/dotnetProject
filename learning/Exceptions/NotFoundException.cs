using System.Net;
using learning.Model.Response;

namespace learning.Exceptions;

public class NotFoundException(string message) : CustomException(HttpStatusCode.NotFound,message) { }