# Potential problems

## Error "No templates matched the input template name" when executing tests

This error occurs, when somehow your local template cache has some problems. It's located in `C:\Users\%username%\.templateengine\dotnetcli\<used .NET Core SDK Version>\`.
To fix it, simple delete the cache. At the next execution, it will be regenerated.