// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Metadata;
using Xunit;

namespace Microsoft.Data.Entity.Tests
{
    public class PropertyExtensionsTest
    {
        [Fact]
        public void Get_generation_property_returns_null_for_property_without_generator()
        {
            var model = new Model();

            var entityType = new EntityType("Entity", model);
            var property = entityType.AddProperty("Property", typeof(int), true);

            Assert.Null(property.GetGenerationProperty());
        }

        [Fact]
        public void Get_generation_property_returns_same_property_on_property_with_generator()
        {
            var model = new Model();

            var entityType = new EntityType("Entity", model);
            var property = entityType.AddProperty("Property", typeof(int), true);

            property.GenerateValueOnAdd = true;

            Assert.Equal(property, property.GetGenerationProperty());
        }

        [Fact]
        public void Get_generation_property_returns_generation_property_from_foreign_key_chain()
        {
            var model = new Model();

            var firstType = new EntityType("First", model);
            var firstProperty = firstType.AddProperty("ID", typeof(int), true);
            var firstKey = firstType.AddKey(firstProperty);

            var secondType = new EntityType("Second", model);
            var secondProperty = secondType.AddProperty("ID", typeof(int), true);
            var secondKey = secondType.AddKey(secondProperty);
            var secondForeignKey = secondType.AddForeignKey(secondProperty, firstKey);

            var thirdType = new EntityType("Third", model);
            var thirdProperty = thirdType.AddProperty("ID", typeof(int), true);
            var thirdForeignKey = thirdType.AddForeignKey(thirdProperty, secondKey);

            firstProperty.GenerateValueOnAdd = true;

            Assert.Equal(firstProperty, thirdProperty.GetGenerationProperty());
        }

        [Fact]
        public void Get_generation_property_returns_generation_property_from_foreign_key_tree()
        {
            var model = new Model();

            var leftType = new EntityType("Left", model);
            var leftId = leftType.AddProperty("Id", typeof(int), true);
            var leftKey = leftType.AddKey(leftId);

            var rightType = new EntityType("Right", model);
            var rightId1 = rightType.AddProperty("Id1", typeof(int), true);
            var rightId2 = rightType.AddProperty("Id2", typeof(int), true);
            var rightKey = rightType.AddKey(new[] { rightId1, rightId2 });

            var middleType = new EntityType("Middle", model);
            var middleProperty1 = middleType.AddProperty("FK1", typeof(int), true);
            var middleProperty2 = middleType.AddProperty("FK2", typeof(int), true);
            var middleKey1 = middleType.AddKey(middleProperty1);
            var middleFK1 = middleType.AddForeignKey(middleProperty1, leftKey);
            var middleFK2 = middleType.AddForeignKey(new[] { middleProperty2, middleProperty1 }, rightKey);

            var endType = new EntityType("End", model);
            var endProperty = endType.AddProperty("FK", typeof(int), true);

            var endFK = endType.AddForeignKey(endProperty, middleKey1);

            rightId2.GenerateValueOnAdd = true;

            Assert.Equal(rightId2, endProperty.GetGenerationProperty());
        }

        [Fact]
        public void Get_generation_property_returns_generation_property_from_foreign_key_graph_with_cycle()
        {
            var model = new Model();

            var leafType = new EntityType("leaf", model);
            var leafId1 = leafType.AddProperty("Id1", typeof(int), true);
            var leafId2 = leafType.AddProperty("Id2", typeof(int), true);
            var leafKey = leafType.AddKey(new[] { leafId1, leafId2 });

            var firstType = new EntityType("First", model);
            var firstId = firstType.AddProperty("Id", typeof(int), true);
            var firstKey = firstType.AddKey(firstId);



            var secondType = new EntityType("Second", model);
            var secondId1 = secondType.AddProperty("Id1", typeof(int), true);
            var secondId2 = secondType.AddProperty("Id2", typeof(int), true);
            var secondKey = secondType.AddKey(secondId1);

            var firstForeignKey = firstType.AddForeignKey(firstId, secondKey);
            var secondForeignKey1 = secondType.AddForeignKey(secondId1, firstKey);
            var secondForeignKey2 = secondType.AddForeignKey(new[] { secondId1, secondId2 }, leafKey);

            leafId1.GenerateValueOnAdd = true;

            Assert.Equal(leafId1, secondId1.GetGenerationProperty());
        }
    }
}
