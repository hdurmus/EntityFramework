﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Utilities
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class LazyRefFacts
    {
        [Fact]
        public async Task Can_initialize_from_multiple_threads_and_initialization_happens_only_once()
        {
            var counter = 0;
            var safeLazy = new LazyRef<string>(() => counter++.ToString());
            var tasks = new List<Task>();

            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => safeLazy.Value));
            }

            await Task.WhenAll(tasks);

            Assert.Equal(1, counter);
        }

        [Fact]
        public async Task Can_exchange_value()
        {
            var safeLazy = new LazyRef<string>(() => "");
            var tasks = new List<Task>();

            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => safeLazy.ExchangeValue(s => s + "s")));
            }

            await Task.WhenAll(tasks);

            Assert.Equal("ssssssssss", safeLazy.Value);
        }

        [Fact]
        public void Has_value_is_false_until_value_accessed()
        {
            var safeLazy = new LazyRef<string>(() => "s");

            Assert.False(safeLazy.HasValue);
            Assert.Equal("s", safeLazy.Value);
            Assert.True(safeLazy.HasValue);
        }
    }
}