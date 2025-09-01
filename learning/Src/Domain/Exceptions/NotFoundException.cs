using System.Net;

namespace learning.Domain.Exceptions;

public class NotFoundException(string message) : CustomException(HttpStatusCode.NotFound,message) { }