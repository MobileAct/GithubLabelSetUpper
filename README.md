# GithubLabelSetUpper
This tool is motivated by [Financial-Times/github-label-sync](https://github.com/Financial-Times/github-label-sync)

## Usage
This tool is published by Github Releases.

### List labels

```
List the all labels

Usage: GithubLabelSetUpper list [options]

Options:
  -h|--help        Show help information
  --host           Host of Github instance, default value is https://github.com
  -t|--token       Token of Github
  -r|--repository  Target repository, value format: {Owner}/{RepositoryName}
  -o|--output      Output file name, including file extension
```

Look up current repository labels and output file.

You can [response file parsing](https://natemcmaster.github.io/CommandLineUtils/docs/response-file-parsing.html?tabs=using-attributes). Make file `list.txt`, example:
```
list
--host
https://github.com
-t
YOUR_GITHUB_TOKEN
-r
REPOSITORY_OWNER/REPOSITORY_NAME
-o
output.yml
```
Execute command on terminal (Mac): `./GithubLabelSetUpper @list.txt`

### SetUp Labels

```
Setup Github labels by option value

Usage: GithubLabelSetUpper setup [options]

Options:
  -h|--help        Show help information
  --host           Host of Github instance, default value is https://github.com
  -t|--token       Token of Github
  -r|--repository  Target repository, value format: {Owner}/{RepositoryName}
  -l|--label       The labels configuration file, support json or yml file.
  -d|--dry-run     Calculate the required label changes, but do not apply Github
```

Look up current repository labels and output file.

You can [response file parsing](https://natemcmaster.github.io/CommandLineUtils/docs/response-file-parsing.html?tabs=using-attributes). Make file `setup.txt`, example:
```
setup
--host
https://github.com
-t
YOUR_GITHUB_TOKEN
-r
REPOSITORY_OWNER/REPOSITORY_NAME
-l
LABEL_FILE
```
Execute command on terminal (Mac): `./GithubLabelSetUpper @setup.txt`

### Label file
Support file type: JSON or YAML

example of yaml:

```yml
- name: 不具合
  color: d73a4a
  description: Something isn't working
  aliases:
    - bug
- name: 機能
  color: a2eeef
  description: New feature
  aliases:
    - enhancement
- name: Feature Request
  color: 7057ff
  description: New feature request
```

The `name` and `color` property is required. `color` property value is color code without `#`.

The `description` property is optional.

The `aliases` property is optional. If this property was set, its label will change to `name` from alias name. So it mean label is kept.

## License
This tool is under MIT License.

This tool is using libraries:

- [Utf8Json](https://github.com/neuecc/Utf8Json) is published by [MIT License](https://github.com/neuecc/Utf8Json/blob/master/LICENSE)
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) is published by [MIT License](https://github.com/aaubry/YamlDotNet/blob/master/LICENSE)
- [Octokit](https://github.com/octokit/octokit.net) is published by [MIT License](https://github.com/octokit/octokit.net/blob/master/LICENSE.txt)
- [CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils) is published by [Apache License v2.0](https://github.com/natemcmaster/CommandLineUtils/blob/master/LICENSE.txt)

## Contribute
ToDo: Write

## Other
Author: [@MeilCli](https://github.com/MeilCli)
