using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Commands.InsertProduct;

internal sealed class InsertProductCommandHandler : ICommandHandler<InsertProductCommand, InsertProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public InsertProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<InsertProductDto>> Handle(InsertProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var res = await _repository
                .InsertProductAsync(new Product() {Name = request.Name, Id = Guid.NewGuid()}, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new Result<InsertProductDto>(new InsertProductDto(res), true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new Result<InsertProductDto>(new InsertProductDto(Guid.Empty),false, e.Message);
        }
    }
}