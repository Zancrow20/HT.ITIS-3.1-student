using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Queries.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsDto>
{
    private readonly IProductRepository _repository;

    public GetProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var res = (await _repository
                .GetAllProductsAsync(cancellationToken)).Select(p => new GetProductDto(p.Id, p.Name));
            return new Result<GetProductsDto>(new GetProductsDto(res), true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new Result<GetProductsDto>(new GetProductsDto(Enumerable.Empty<GetProductDto>()),
                false, e.Message);
        }
    }
}