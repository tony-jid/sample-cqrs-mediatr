using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace SampleCQRSMediatR.PipelineBehaviros
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<TRequest>(request); // Getting validators of the request from FlientValidation.ValidationContext
            var failures = _validators
                .Select(async x => await x.ValidateAsync(context, cancellationToken)) // Performing async validation
                .SelectMany(x => x.Result.Errors) // Flattening all error lists of all ValidationResults
                .Where(x => x != null)
                .ToList();

            if (failures != null && failures.Any())
            {
                throw new ValidationException(failures);
            }

            return next();
        }
    }
}
