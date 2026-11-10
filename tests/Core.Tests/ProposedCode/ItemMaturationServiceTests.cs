//-----------------------------------------------------------------------
// <copyright file="ItemMaturationServiceTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using FluentAssertions;
using GildedRoseKata.OriginalCode;

public class ItemMaturationServiceTests
{
    [Theory]
    [InlineData("Elixir of the Mongoose", 10, 5, 9, 4)]
    [InlineData("Aged Brie", 10, 2, 11, 1)]
    [InlineData("AgedBrie", 10, 2, 11, 1)]
    [InlineData("Backstage passes to a TAFKAL80ETC concert", 20, 10, 22, 9)]
    [InlineData("Backstage", 20, 5, 23, 4)]
    [InlineData("Conjured Mana Cake", 10, 5, 8, 4)]
    [InlineData("Conjured", 10, 0, 6, -1)]
    [InlineData("Sulfuras, Hand of Ragnaros", 80, 0, 80, 0)]
    [InlineData("Sulfuras, Hand of Ragnaros", 10, 3, 80, 3)]
    [InlineData("Backstage", 20, -1, 0, -2)]
    [InlineData("Elixir of the Mongoose", 10, -1, 8, -2)]
    [InlineData("Elixir of the Mongoose", 60, 5, 49, 4)]
    [InlineData("Elixir of the Mongoose", -5, 5, 0, 4)]
    [InlineData("", 10, 5, 9, 4)]
    [InlineData(null, 10, 5, 9, 4)]
    public void Mature_UsesExpectedRulesForResolvedItemKinds(string? name, int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var service = new ItemMaturationService();
        var item = new Item
        {
            Name = name!,
            Quality = quality,
            SellIn = sellIn,
        };

        var result = service.Mature(item);

        result.Quality.Should().Be(expectedQuality);
        result.SellIn.Should().Be(expectedSellIn);
    }

    [Theory]
    [InlineData("Elixir of the Mongoose")]
    [InlineData("Aged Brie")]
    [InlineData("Sulfuras, Hand of Ragnaros")]
    [InlineData("A custom item name")]
    [InlineData("")]
    [InlineData(null)]
    public void Mature_PreservesName(string? name)
    {
        var service = new ItemMaturationService();
        var item = new Item { Name = name!, Quality = 10, SellIn = 5 };

        var result = service.Mature(item);

        result.Name.Should().Be(name);
    }

    [Fact]
    public void Mature_DoesNotMutateInputItem()
    {
        var service = new ItemMaturationService();
        var item = new Item { Name = "Aged Brie", Quality = 10, SellIn = 2 };

        var result = service.Mature(item);

        result.Should().NotBeSameAs(item);
        item.Name.Should().Be("Aged Brie");
        item.Quality.Should().Be(10);
        item.SellIn.Should().Be(2);
    }

    [Fact]
    public void Mature_NullItem_ThrowsNullReferenceException()
    {
        var service = new ItemMaturationService();

        var act = () => service.Mature(null!);

        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Mature_WithInjectedPolicy_PassesResolvedKindAndStateAndUsesReturnedState()
    {
        var returnedState = new ItemState(new Quality(33), new SellIn(7));
        var policy = new StubMaturingPolicy(returnedState);
        var service = new ItemMaturationService(policy);
        var item = new Item { Name = "Aged Brie", Quality = 10, SellIn = 2 };

        var result = service.Mature(item);

        policy.CallCount.Should().Be(1);
        policy.ReceivedKind.Should().BeOfType<ItemKind.AgedBrie>();
        policy.ReceivedState.Quality.Value.Should().Be(10);
        policy.ReceivedState.SellIn.Value.Should().Be(2);
        result.Name.Should().Be("Aged Brie");
        result.Quality.Should().Be(33);
        result.SellIn.Should().Be(7);
    }

    private sealed class StubMaturingPolicy : IMaturingPolicy
    {
        private readonly ItemState stateToReturn;

        public StubMaturingPolicy(ItemState stateToReturn)
        {
            this.stateToReturn = stateToReturn;
        }

        public int CallCount { get; private set; }

        public ItemKind? ReceivedKind { get; private set; }

        public ItemState ReceivedState { get; private set; }

        public ItemState Mature(ItemKind kind, ItemState state)
        {
            this.CallCount++;
            this.ReceivedKind = kind;
            this.ReceivedState = state;

            return this.stateToReturn;
        }
    }
}
