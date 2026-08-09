# Contributing to d2lang-cs

Thanks for helping improve `d2lang-cs`. Focused pull requests with tests are the easiest to review and release safely.

## Development setup

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0). The D2 CLI is optional for normal library development and required when checking generated D2 syntax locally. CI currently uses D2 CLI `v0.7.1`.

Fork and clone the repository, then create a branch from `main`:

```bash
git switch main
git pull --ff-only
git switch -c feature/short-description
```

Restore, build, and test from the repository root:

```bash
dotnet restore d2lang-cs.slnx
dotnet build d2lang-cs.slnx --configuration Release --no-restore
dotnet test test/Tests.csproj --configuration Release --no-build
```

Create the NuGet package with SDK package validation enabled:

```bash
dotnet pack src/d2lang-cs.csproj \
  --configuration Release \
  --output artifacts/packages \
  -p:EnablePackageValidation=true
```

## Validate generated D2

Install the [D2 CLI](https://d2lang.com/tour/install/), run the example, and validate its output:

```bash
dotnet run --project example/cli/d2-sample-cli.csproj \
  --configuration Release > /tmp/d2lang-cs-example.d2
d2 validate /tmp/d2lang-cs-example.d2
```

When changing serialization, add tests for both the exact emitted source and parser acceptance. Include cases with reserved characters, quotes, whitespace, multiline text, URLs, and nested containers where relevant.

## Pull requests

- Keep changes focused on one concern.
- Add or update tests for behavior changes.
- Update the README when public behavior, requirements, or compatibility changes.
- Avoid unrelated formatting or generated-file changes.
- Run the release build, tests, package validation, and relevant D2 validation before requesting review.
- Call out intentional compatibility or output-format changes in the description.

Pull requests run clean builds and tests on Linux and Windows. CI also publishes test results, Cobertura coverage artifacts, a coverage summary, a validated NuGet package artifact, and the example D2 source validation result.

## Release process

Releases are published by `.github/workflows/release.yml`. Maintainers should:

1. Confirm the intended commit is on `main` and CI is green.
2. Confirm the `nuget.org` GitHub environment is protected as desired and contains a `NUGET_API_KEY` secret scoped to the `d2lang-cs` package.
3. Choose an unused semantic version such as `1.2.3` or `1.2.3-rc.1`.
4. Create an annotated `v`-prefixed tag and push it:

   ```bash
   git tag -a v1.2.3 -m "d2lang-cs 1.2.3"
   git push origin v1.2.3
   ```

5. Review the `Publish NuGet package` workflow and its package artifact before confirming the package on NuGet.org.

The workflow derives `PackageVersion` from the tag, repeats tests, enables SDK package validation, creates `.nupkg` and `.snupkg` artifacts, and publishes with `dotnet nuget push`. Publishing uses read-only repository permissions and exposes `NUGET_API_KEY` only to the final push step. Duplicate versions are skipped safely, but NuGet package versions are immutable; use a new version if published contents need to change.
