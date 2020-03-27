Unity Test Package
===

## Usage

- Edit your Unity projects "Packages/manifest.json" to include the string 
  `"io.github.cnheider.unity-test-package": "https://github.com/cnheider/unity-test-package.git"}`.
  
  Example `manifest.json`
  ````
  {
    "dependencies": {
      "com.unity.package-manager-ui": "0.0.0-builtin",
      ...
      "io.github.cnheider.unity-test-package": "https://github.com/cnheider/unity-test-package.git",
    }
  }
  ````
  You can use `"io.github.cnheider.unity-test-package": "https://github.com/cnheider/unity-test-package.git#branch"` for a specific branch.
