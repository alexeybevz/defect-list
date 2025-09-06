using System;
using System.Collections.Generic;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;

namespace DefectListBusinessLogic.Fakes
{
    public class FakeAsupBomContextFactory : IAsupBomContextFactory
    {
        public List<AsupBomComponentDto> AskBase(string rootProductName, decimal rootProductQty, DateTime? specifOnDate = null)
        {
            var newBom = new List<AsupBomComponentDto>()
            {
                new AsupBomComponentDto()
                {
                    detals = "ИЗДЕЛИЕРЕМОНТ",
                    detal = "ИЗДЕЛИЕ_РЕМОНТ",
                    detalInitial = "ИЗДЕЛИЕ",
                    imadetal = "ИЗДЕЛИЕ_РЕМОНТ",
                    Detal_Product_Id = 0,
                    Detal_Code_LSF82 = 0,
                    DetalCodeSL = "0",
                    typ = "сб.ед",
                    plan_kol = 1,
                    units = "шт",

                    StructureNumber = "1"
                },
                new AsupBomComponentDto()
                {
                    uzels = "ИЗДЕЛИЕРЕМОНТ",
                    uzel = "ИЗДЕЛИЕ_РЕМОНТ",
                    Uzel_Product_Id = 0,
                    Uzel_Code_LSF82 = 0,
                    UzelCodeSL = "0",

                    detals = "ДСЕ1",
                    detal = "ДСЕ1",
                    detalInitial = "ДСЕ1",
                    imadetal = "ДСЕ1",
                    Detal_Product_Id = 1,
                    Detal_Code_LSF82 = 1,
                    DetalCodeSL = "1",
                    typ = "сб.ед",
                    plan_kol = 1,
                    units = "шт",

                    StructureNumber = "1.1"
                },
                new AsupBomComponentDto()
                {
                    uzels = "ИЗДЕЛИЕРЕМОНТ",
                    uzel = "ИЗДЕЛИЕ_РЕМОНТ",
                    Uzel_Product_Id = 0,
                    Uzel_Code_LSF82 = 0,
                    UzelCodeSL = "0",

                    detals = "ДСЕ2",
                    detal = "ДСЕ2",
                    detalInitial = "ДСЕ2",
                    imadetal = "ДСЕ2",
                    Detal_Product_Id = 2,
                    Detal_Code_LSF82 = 2,
                    DetalCodeSL = "2",
                    typ = "сб.ед",
                    plan_kol = 2,
                    units = "шт",

                    StructureNumber = "1.2"
                },
                new AsupBomComponentDto()
                {
                    uzels = "ДСЕ2",
                    uzel = "ДСЕ2",
                    Uzel_Product_Id = 2,
                    Uzel_Code_LSF82 = 2,
                    UzelCodeSL = "2",

                    detals = "ДСЕ3",
                    detal = "ДСЕ3",
                    detalInitial = "ДСЕ3",
                    imadetal = "ДСЕ3",
                    Detal_Product_Id = 3,
                    Detal_Code_LSF82 = 3,
                    DetalCodeSL = "3",
                    typ = "дет",
                    plan_kol = 1,
                    units = "шт",

                    StructureNumber = "1.2.1"
                },
                new AsupBomComponentDto()
                {
                    uzels = "ДСЕ2",
                    uzel = "ДСЕ2",
                    Uzel_Product_Id = 2,
                    Uzel_Code_LSF82 = 2,
                    UzelCodeSL = "2",

                    detals = "ДСЕ4",
                    detal = "ДСЕ4",
                    detalInitial = "ДСЕ4",
                    imadetal = "ДСЕ4",
                    Detal_Product_Id = 4,
                    Detal_Code_LSF82 = 4,
                    DetalCodeSL = "4",
                    typ = "дет",
                    plan_kol = 1,
                    units = "шт",

                    StructureNumber = "1.2.2"
                },
                new AsupBomComponentDto()
                {
                    uzels = "ДСЕ1",
                    uzel = "ДСЕ1",
                    Uzel_Product_Id = 1,
                    Uzel_Code_LSF82 = 1,
                    UzelCodeSL = "1",

                    detals = "ДСЕ5",
                    detal = "ДСЕ5",
                    detalInitial = "ДСЕ5",
                    imadetal = "ДСЕ5",
                    Detal_Product_Id = 5,
                    Detal_Code_LSF82 = 5,
                    DetalCodeSL = "5",
                    typ = "покуп",
                    plan_kol = 3,
                    units = "шт",

                    StructureNumber = "1.1.1"
                },
            };

            return newBom;
        }
    }
}