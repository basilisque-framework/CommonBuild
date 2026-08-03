/*
   Copyright 2026 Alexander Stärk

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

namespace Consumer.Shared;

/// <summary>
/// Classes implementing this interface provide information for integration tests.
/// </summary>
public interface ITestInfo
{
    /// <summary>
    /// Gets a value indicating whether the test fixture assembly should be packable.
    /// </summary>
    bool IsPackable { get; }

    /// <summary>
    /// Gets a value indicating whether the test fixture assembly should be publishable.
    /// </summary>
    bool IsPublishable { get; }

    /// <summary>
    /// Gets the assembly of the test fixture.
    /// </summary>
    System.Reflection.Assembly Assembly => this.GetType().Assembly;
}
