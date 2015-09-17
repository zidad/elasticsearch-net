﻿using System;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IExplainRequest : IRequest<ExplainRequestParameters>
	{
		[JsonProperty("query")]
		IQueryContainer Query { get; set; }
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IExplainRequest<T> : IExplainRequest
		where T : class
	{ }

	internal static class ExplainPathInfo
	{
		public static void Update(RequestPath<ExplainRequestParameters> pathInfo, IExplainRequest request)
		{
			var source = request.Parameters.GetQueryStringValue<string>("source");
			var q = request.Parameters.GetQueryStringValue<string>("q");
			pathInfo.HttpMethod = (!source.IsNullOrEmpty() || !q.IsNullOrEmpty())
				? HttpMethod.GET
				: HttpMethod.POST;
		}
	}

	public partial class ExplainRequest : RequestBase<ExplainRequestParameters>, IExplainRequest
	{
		public IQueryContainer Query { get; set; }

		protected override void UpdateRequestPath(IConnectionSettingsValues settings, RequestPath<ExplainRequestParameters> pathInfo)
		{
			ExplainPathInfo.Update(pathInfo, this);
		}
	}

	public partial class ExplainRequest<T> : RequestBase<ExplainRequestParameters>, IExplainRequest<T>
		where T : class
	{
		public IQueryContainer Query { get; set; }

		protected override void UpdateRequestPath(IConnectionSettingsValues settings, RequestPath<ExplainRequestParameters> pathInfo)
		{
			ExplainPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("Explain")]
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public partial class ExplainDescriptor<T> : RequestDescriptorBase<ExplainDescriptor<T>, ExplainRequestParameters>, IExplainRequest<T>
		where T : class
	{
		private IExplainRequest Self => this;

		IQueryContainer IExplainRequest.Query { get; set; }

		public ExplainDescriptor<T> Query(Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
		{
			Self.Query = querySelector(new QueryContainerDescriptor<T>());
			return this;
		}

		protected override void UpdateRequestPath(IConnectionSettingsValues settings, RequestPath<ExplainRequestParameters> pathInfo)
		{
			ExplainPathInfo.Update(pathInfo, this);
		}
	}
}
