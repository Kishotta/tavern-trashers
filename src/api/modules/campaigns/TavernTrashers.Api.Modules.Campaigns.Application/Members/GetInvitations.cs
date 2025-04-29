using Dapper;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Data;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Campaigns.Application.Campaigns;

namespace TavernTrashers.Api.Modules.Campaigns.Application.Members;

public sealed record GetInvitationsQuery : IQuery<IReadOnlyCollection<MyInvitationResponse>>;

internal sealed class GetInvitationsQueryHandler(
	IClaimsProvider claims,
	IDbConnectionFactory dbConnectionFactory)
	: IQueryHandler<GetInvitationsQuery, IReadOnlyCollection<MyInvitationResponse>>
{
	// public async Task<Result<IReadOnlyCollection<InvitationResponse>>> Handle(
	// 	GetInvitationsQuery request,
	// 	CancellationToken cancellationToken) =>
	// 	await campaignRepository.GetMemberInvitationsReadOnlyAsync(cancellationToken)
	// 	   .TransformAsync(invitations => invitations
	// 		   .Select(invitation => (InvitationResponse)invitation)
	// 		   .ToList()
	// 		   .AsReadOnly())

	public async Task<Result<IReadOnlyCollection<MyInvitationResponse>>> Handle(
		GetInvitationsQuery request,
		CancellationToken cancellationToken)
	{
		await using var dbConnection = await dbConnectionFactory.OpenConnectionAsync();

		const string sql = $"""
		                     SELECT
		                     	ci.id AS {nameof(MyInvitationResponse.Id)},
		                     	ci.email AS {nameof(MyInvitationResponse.Email)},
		                     	c.id AS {nameof(MyInvitationResponse.CampaignId)},
		                     	c.title AS {nameof(MyInvitationResponse.CampaignTitle)},
		                     	ci.role AS {nameof(MyInvitationResponse.Role)}
		                     FROM campaigns.campaigns c
		                     INNER JOIN campaigns.campaign_invitations ci ON ci.campaign_id = c.id
		                     WHERE ci.email = @Email
		                    """;

		var invitations = await dbConnection.QueryAsync<MyInvitationResponse>(
			sql, new { Email = claims.GetEmail() });

		return invitations
		   .ToList()
		   .AsReadOnly();
	}
}