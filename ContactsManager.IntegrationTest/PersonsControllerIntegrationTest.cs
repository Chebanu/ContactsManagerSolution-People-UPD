﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace CRUDTests
{
    public class PersonsControllerIntegrationTest : IClassFixture<Web_App_Factory>
    {

        private readonly HttpClient _client;
        public PersonsControllerIntegrationTest(Web_App_Factory factory)
        {
            _client = factory.CreateClient();
        }

        #region Index


        [Fact]
        public async Task Index_ToReturnView()
        {
            //Arrange

            //Act
           HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            //Assert

            response.Should().BeSuccessful();

            string responseBody = await response.Content.ReadAsStringAsync();
            
            HtmlDocument html = new HtmlDocument();

            html.LoadHtml(responseBody);
            var document = html.DocumentNode;

            document.QuerySelectorAll("table.persons").Should().NotBeNull();
        }

        #endregion
    }
}
