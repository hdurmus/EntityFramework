﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Storage
{
    public class DataStoreErrorLogState
    {
        public DataStoreErrorLogState([NotNull] Type contextType)
        {
            Check.NotNull(contextType, "contextType");

            ContextType = contextType;
        }

        public virtual Type ContextType { get; }
    }
}
