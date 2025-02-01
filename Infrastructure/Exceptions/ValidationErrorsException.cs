using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions;
public class ValidationErrorsException : Exception
{
	public List<string> Errors { get; }
	public ValidationErrorsException(List<string> errors)
	{
		Errors = errors;
	}
}
