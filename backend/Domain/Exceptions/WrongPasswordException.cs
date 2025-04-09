namespace Domain.Exceptions;

public class WrongPasswordException(string message) : Exception(message);