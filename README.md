# slack-bot-hive
A prototype productivity app for Slack.
Originally created as part of a hack-day idea at Madgex.

### What purpose does slack-bot-hive serve?
A large amount of knowledge within the organization is considered 'tribal': unwritten rules and information known by few individuals that is constantly being accumulated. These pieces of information are usually memorized or written down on paper so although they have the potential to bring value to another, little or none of it is shared. Platforms like Confluence or Sharepoint as a central repository of data work well when utilized but can be impractical or cumbersome for recording small tidbits of data which would either not warrant a full page or deemed not worth the time to be recorded. (Plus people are lazy!) These tidbits can be written down by their collector which provides a reliable reference for the future, but requires time-consuming manual lookup. A digital file can utilize much faster lookup times but files are not often shared and still require regular updating.

A better solution would be to use an existing rapidly accessible platform, like Slack, to store tidbits and retrieve data about a particular issue. The information can be held centrally as to be cross-functional and shared by all, with searches favoring one's team or role. For data storage, the ELK stack can be used (Elasticsearch, Kibana, Logstash) as search and indexing operations are already baked-in. A server handles the processing between the two.

### Architecture
The AskMadge application is a .Net Core project running containerised in Docker. Storage of data is provided by an Elasticsearch stack service in AWS (can be substituted by any other ES instance). Slack events and slack actions triggered by users are passed through from the slack platform to the dotnet core API to be action-ed upon. 

Programmatic connectivity to Slack is accomplished by using the [Slack.Api.CSharp](https://github.com/JamesMarcogliese/slack-api-csharp) NuGet package. 

Programmatic connectivity to ES is accomplished by using the [NEST](https://github.com/elastic/elasticsearch-net) NuGet package.

![](https://user-images.githubusercontent.com/8539492/58374560-5c84fd80-7f0e-11e9-9dfa-67b981637de0.png "arch")

### Screenshots
<p align="center">
 <img src="https://user-images.githubusercontent.com/8539492/37557057-3ad23dea-29d5-11e8-9a31-28dd64d1bf0b.PNG" width="200"/>
 <img src="https://user-images.githubusercontent.com/8539492/37557058-3ae2ac8e-29d5-11e8-8dbb-509c2eda2893.PNG" width="200"/>
 <img src="https://user-images.githubusercontent.com/8539492/37557059-3af1d268-29d5-11e8-9bd0-82f1b60d3958.PNG" width="200"/>
 <img src="https://user-images.githubusercontent.com/8539492/37557060-3b01731c-29d5-11e8-8be3-718963e12265.PNG" width="200"/>
 <img src="https://user-images.githubusercontent.com/8539492/37557061-3b1182f2-29d5-11e8-9fdd-2e76e2d2f8c9.PNG" width="200"/>
</p>

### Setup
Please see the wiki for Slack bot setup instructions.
