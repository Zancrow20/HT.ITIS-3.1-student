using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Products.Commands.InsertProduct;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Commands.UpdateProduct;

internal sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }
    
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _repository
                .UpdateProductAsync(new Product() {Name = request.Name, Id = request.Guid}, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new Result( true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new Result(false, e.Message);
        }
    }
}