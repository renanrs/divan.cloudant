using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;

namespace Divan.Cloudant.Exceptions
{
    public sealed class DatabaseNotFoundException : Exception
    {
        public DatabaseNotFoundException()
        {

        }

        public DatabaseNotFoundException(string message)
            : base(message)
        {

        }

        public DatabaseNotFoundException(string message, Exception inner)
           : base(message,inner)
        {

        }

    }

    public sealed class DocumentNotFoundException:Exception
    {
        public DocumentNotFoundException()
        {
            
        }

        public DocumentNotFoundException(string message)
        :base(message)
        {
            
        }

        public DocumentNotFoundException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

    public sealed class PreconditionFailedException:Exception
    {
        public PreconditionFailedException()
        {
            
        }

        public PreconditionFailedException(string message)
        :base(message)
        {
            
        }

        public PreconditionFailedException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

     public sealed class DocumentConflictException:Exception
    {
        public DocumentConflictException()
        {
            
        }

        public DocumentConflictException(string message)
        :base(message)
        {
            
        }

        public DocumentConflictException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

    public sealed class RequestEntityTooLargeException:Exception
    {
        public RequestEntityTooLargeException()
        {
            
        }

        public RequestEntityTooLargeException(string message)
        :base(message)
        {
            
        }

        public RequestEntityTooLargeException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

    public sealed class ExpectationFailedException:Exception
    {
        public ExpectationFailedException()
        {
            
        }

        public ExpectationFailedException(string message)
        :base(message)
        {
            
        }

        public ExpectationFailedException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

    public sealed class InternalServerErrorException:Exception
    {
        public InternalServerErrorException()
        {
            
        }

        public InternalServerErrorException(string message)
        :base(message)
        {
            
        }

        public InternalServerErrorException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

     public sealed class ServiceUnavailableException:Exception
    {
        public ServiceUnavailableException()
        {
            
        }

        public ServiceUnavailableException(string message)
        :base(message)
        {
            
        }

        public ServiceUnavailableException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

    public sealed class PaymentRequiredException:Exception
    {
        public PaymentRequiredException()
        {
            
        }

        public PaymentRequiredException(string message)
        :base(message)
        {
            
        }

        public PaymentRequiredException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

    public sealed class ForbiddenException:Exception
    {
        public ForbiddenException()
        {
            
        }

        public ForbiddenException(string message)
        :base(message)
        {
            
        }

        public ForbiddenException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

    public sealed class UnauthorizedException:Exception
    {
        public UnauthorizedException()
        {
            
        }

        public UnauthorizedException(string message)
        :base(message)
        {
            
        }

        public UnauthorizedException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }

    public sealed class NotModifiedException:Exception
    {
        public NotModifiedException()
        {
            
        }

        public NotModifiedException(string message)
        :base(message)
        {
            
        }

        public NotModifiedException(string message,Exception inner)
        :base(message,inner)
        {
            
        }
    }


    public static class ExceptionHelper
    {
        public static void ThrowDbException(HttpStatusCode status)
        {
            switch (status)
            {                
                case HttpStatusCode.NotFound:
                    throw new DatabaseNotFoundException();
            }

            GenericExceptions(status);
        }

        /// <summary>
        /// Method throw an exception if it matches with one HttpStatusCode
        /// </summary>
        /// <param name="status">HttpStatusCode</param>
        public static void ThrowDocumentException(HttpStatusCode status)
        {
            switch (status)
            {
                case HttpStatusCode.Conflict:
                    throw new DocumentConflictException(status.ToString());
                case HttpStatusCode.NotFound:
                    throw new DocumentNotFoundException(status.ToString());
                case HttpStatusCode.RequestEntityTooLarge:
                    throw new RequestEntityTooLargeException(status.ToString());
                case HttpStatusCode.ExpectationFailed:
                    throw new ExpectationFailedException(status.ToString());  
                case HttpStatusCode.NotModified:
                    throw new NotModifiedException(status.ToString());                   

            }

            GenericExceptions(status);
        }

        private static void GenericExceptions(HttpStatusCode status)
        {
            switch (status)
            {
                case HttpStatusCode.InternalServerError:
                    throw new InternalServerErrorException(status.ToString());
                case HttpStatusCode.ServiceUnavailable:
                    throw new ServiceUnavailableException(status.ToString());
                case HttpStatusCode.PaymentRequired:
                    throw new PaymentRequiredException(status.ToString());
                case HttpStatusCode.Forbidden:
                    throw new ForbiddenException(status.ToString());
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException(status.ToString());   
                case HttpStatusCode.PreconditionFailed:
                    throw new PreconditionFailedException(status.ToString());  
                default:
                    if(status != HttpStatusCode.Accepted && 
                    status != HttpStatusCode.Created && 
                    status != HttpStatusCode.OK){
                        throw new Exception(status.ToString());
                    }  
                    break;                          

            }
        }
    }
}
