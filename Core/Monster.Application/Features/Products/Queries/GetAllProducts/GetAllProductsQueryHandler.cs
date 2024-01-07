using MediatR;
using Microsoft.EntityFrameworkCore;
using Monster.Application.DTOs;
using Monster.Application.Interfaces.AutoMapper;
using Monster.Application.Interfaces.UnitOfWorks;
using Monster.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQueryRequest, IList<GetAllProductsQueryResponse>>
    {
        private readonly IUnitOfWorks unitOfWorks;
        private readonly IMapper mapper;

        public GetAllProductsQueryHandler(IUnitOfWorks unitOfWorks, IMapper mapper)
        {
            this.unitOfWorks = unitOfWorks;
            this.mapper = mapper;
        }




        public async Task<IList<GetAllProductsQueryResponse>> Handle(GetAllProductsQueryRequest request, CancellationToken cancellationToken)
        {
            var products = await unitOfWorks.GetReadRepository<Product>().GetAllAsync(include: x => x.Include(b => b.Brand));

            var brand = mapper.Map<BrandDto, Brand>(new Brand());

            // List<GetAllProductsQueryResponse> response = new();

            //foreach (var product in products)
            //    response.Add(new GetAllProductsQueryResponse
            //    {
            //        Title = product.Title,
            //        Description = product.Description,
            //        Discount = product.Discount,
            //        Price = product.Price - (product.Price * product.Discount / 100),
            //    });

            var map = mapper.Map<GetAllProductsQueryResponse, Product>(products);
            foreach (var item in map)
                item.Price -= (item.Price * item.Discount / 100);

            // return map;
            throw new Exception("Hata:");

        }
    }
}
