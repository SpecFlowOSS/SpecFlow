using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Specs.StepDefinitions
{
    [Binding]
    public class PricingStepDefinitions
    {
        class PriceCalculator
        {
            private readonly Dictionary<string, int> _basket = new();
            private readonly Dictionary<string, decimal> _itemPrices = new();

            public void AddToBasket(string productName, int quantity)
            {
                if (!_basket.TryGetValue(productName, out var currentQuantity)) 
                    currentQuantity = 0;
                _basket[productName] = currentQuantity + quantity;
            }

            public decimal CalculatePrice()
            {
                return _basket.Sum(bi => GetPrice(bi.Key) * bi.Value);
            }

            private decimal GetPrice(string productName)
            {
                if (_itemPrices.TryGetValue(productName, out var itemPrice)) 
                    return itemPrice;
                return 1.5m;
            }

            public void SetPrice(string productName, in decimal itemPrice)
            {
                _itemPrices[productName] = itemPrice;
            }
        }

        private readonly ScenarioContext _scenarioContext;
        private readonly PriceCalculator _priceCalculator = new();
        private decimal _calculatedPrice;

        public PricingStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"the price of (.*) is €(.*)")]
        public void GivenThePriceOfProductIs(string productName, decimal itemPrice)
        {
            _priceCalculator.SetPrice(productName, itemPrice);
        }

        [Given(@"the customer has put (.*) pcs of (.*) to the basket")]
        public void GivenTheCustomerHasPutPcsOfProductToTheBasket(int quantity, string productName)
        {
            _priceCalculator.AddToBasket(productName, quantity);
        }

        [Given(@"the customer has put a product to the basket")]
        public void GivenTheCustomerHasPutAProductToTheBasket()
        {
            var productName = _scenarioContext.ScenarioInfo.Arguments["product"]?.ToString();
            _priceCalculator.AddToBasket(productName, 1);
        }

        [When(@"the basket price is calculated")]
        public void WhenTheBasketPriceIsCalculated()
        {
            _calculatedPrice = _priceCalculator.CalculatePrice();
        }

        [Then(@"the basket price should be greater than zero")]
        public void ThenTheBasketPriceShouldBeGreaterThanZero()
        {
            Assert.Greater(_calculatedPrice, 0);
        }

        [Then(@"the basket price should be €(.*)")]
        public void ThenTheBasketPriceShouldBe(decimal expectedPrice)
        {
            Assert.AreEqual(expectedPrice, _calculatedPrice);
        }

    }
}
