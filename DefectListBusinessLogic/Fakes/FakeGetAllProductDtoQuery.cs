using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeGetAllProductDtoQuery : IGetAllProductDtoQuery
    {
        public Task<IEnumerable<ProductDto>> Execute()
        {
            return Task.FromResult(data.AsEnumerable());
        }

        public Task<IEnumerable<ProductDto>> ExecuteDesignSpecification()
        {
            return Task.FromResult(data.AsEnumerable());
        }

        public Task<ProductDto> ExecuteByProductId(int productId)
        {
            var fakeData = data.FirstOrDefault(x => x.ProductId == productId);
            return Task.FromResult(fakeData);
        }

        public Task<ProductDto> ExecuteByCodeLsf82(int codeLsf82)
        {
            var fakeData = data.FirstOrDefault(x => x.CodeLsf82 == codeLsf82);
            return Task.FromResult(fakeData);
        }

        public Task<ProductDto> ExecuteByDetals(string detals)
        {
            var fakeData = data.FirstOrDefault(x => x.Detals == detals);
            return Task.FromResult(fakeData);
        }

        private readonly List<ProductDto> data = new List<ProductDto>()
            {
                new ProductDto()
                {
                    Detals = "ИЗДЕЛИЕРЕМОНТ",
                    Name = "ИЗДЕЛИЕ_РЕМОНТ",
                    ExtName = "ИЗДЕЛИЕ_РЕМОНТ",
                    Type = "издел",
                    Um = "шт",
                    Id = 0,
                    ProductId = 0,
                    CodeLsf82 = 0,
                    CodeErp = "0",
                    ProductModes = new List<ProductModeDto>() { new ProductModeDto() { Name = "Конструкторская спецификация" }}
                },
                new ProductDto()
                {
                    Detals = "ДСЕ1",
                    Name = "ДСЕ1",
                    ExtName = "ДСЕ1",
                    Type = "сб.ед",
                    Um = "шт",
                    Id = 1,
                    ProductId = 1,
                    CodeLsf82 = 1,
                    CodeErp = "1",
                    ProductModes = new List<ProductModeDto>() { new ProductModeDto() { Name = "Конструкторская спецификация" }}
                },
                new ProductDto()
                {
                    Detals = "ДСЕ2",
                    Name = "ДСЕ2",
                    ExtName = "ДСЕ2",
                    Type = "сб.ед",
                    Um = "шт",
                    Id = 2,
                    ProductId = 2,
                    CodeLsf82 = 2,
                    CodeErp = "2",
                    ProductModes = new List<ProductModeDto>() { new ProductModeDto() { Name = "Конструкторская спецификация" }}
                },
                new ProductDto()
                {
                    Detals = "ДСЕ3",
                    Name = "ДСЕ3",
                    ExtName = "ДСЕ3",
                    Type = "дет",
                    Um = "шт",
                    Id = 3,
                    ProductId = 3,
                    CodeLsf82 = 3,
                    CodeErp = "3",
                    ProductModes = new List<ProductModeDto>() { new ProductModeDto() { Name = "Конструкторская спецификация" }}
                },
                new ProductDto()
                {
                    Detals = "ДСЕ4",
                    Name = "ДСЕ4",
                    ExtName = "ДСЕ4",
                    Type = "дет",
                    Um = "шт",
                    Id = 4,
                    ProductId = 4,
                    CodeLsf82 = 4,
                    CodeErp = "4",
                    ProductModes = new List<ProductModeDto>() { new ProductModeDto() { Name = "Конструкторская спецификация" }}
                },
                new ProductDto()
                {
                    Detals = "ДСЕ5",
                    Name = "ДСЕ5",
                    ExtName = "ДСЕ5",
                    Type = "покуп",
                    Um = "шт",
                    Id = 5,
                    ProductId = 5,
                    CodeLsf82 = 5,
                    CodeErp = "5",
                    ProductModes = new List<ProductModeDto>() { new ProductModeDto() { Name = "Конструкторская спецификация" }}
                },
                new ProductDto()
                {
                    Detals = "ДСЕ3Р",
                    Name = "ДСЕ3Р",
                    ExtName = "ДСЕ3Р",
                    Type = "дет",
                    Um = "шт",
                    Id = 6,
                    ProductId = 6,
                    CodeLsf82 = 6,
                    CodeErp = "6",
                    ProductModes = new List<ProductModeDto>() { new ProductModeDto() { Name = "Конструкторская спецификация" }}
                },
            };
    }
}