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

namespace CommonBuild.Integration.TestSupport.TestSetup;

internal class UnpackNuGetPackages
{
    [Before(TestSession)]
    public static void BeforeTestSession()
    {
        //clean old runs
        cleanup();

        //unpack the new packages
        unpack();
    }

    [After(TestSession)]
    public static void AfterTestSession()
    {
        cleanup();
    }

    private static void unpack()
    {

    }

    private static void cleanup()
    {
    }
}
