﻿using TorControlClientNet.Helper;

namespace TorControlClientNet.Constants
{
    public enum TorCommands
    {
        AUTHENTICATE,
        GETCONF,
        GETINFO,
        SETEVENTS,
        QUIT
    }

    public enum TorGetInfoKeywords
    {
        [StringValue("version")] version,
        [StringValue("config-file")] configfile,
        [StringValue("config-defaults-file")] configdefaultsfile,
        [StringValue("config-text")] configtext,
        [StringValue("exit-policy/default")] exitpolicydefault,

        [StringValue("exit-policy/reject-private/default")]
        exitpolicyrejectprivatedefault,

        [StringValue("exit-policy/reject-private/relay")]
        exitpolicyrejectprivaterelay,
        [StringValue("exit-policy/ipv4")] exitpolicyipv4,
        [StringValue("exit-policy/ipv6")] exitpolicyipv6,
        [StringValue("exit-policy/full")] exitpolicyfull,
        [StringValue("desc/id/")] descidORidentity,
        [StringValue("desc/name/")] descnameORnickname,
        [StringValue("md/id/")] mdidORidentity,
        [StringValue("desc/download-enabled")] descdownloadenabled,
        [StringValue("md/download-enabled")] mddownloadenabled,
        [StringValue("dormant")] dormant,
        [StringValue("desc-annotations/id/")] descannotationsidORidentity,
        [StringValue("extra-info/digest/")] extrainfodigestdigest,
        [StringValue("ns/id/")] nsidORidentity,
        [StringValue("ns/all")] nsall,
        [StringValue("ns/purpose/")] nspurposepurpose,
        [StringValue("desc/all-recent")] descallrecent,
        [StringValue("network-status")] networkstatus,
        [StringValue("address-mappings/all")] addressmappingsall,

        [StringValue("address-mappings/config")]
        addressmappingsconfig,

        [StringValue("address-mappings/cache")]
        addressmappingscache,

        [StringValue("address-mappings/control")]
        addressmappingscontrol,
        [StringValue("addr-mappings/*")] addrmappings,
        [StringValue("address")] address,
        [StringValue("fingerprint")] fingerprint,
        [StringValue("circuit-status")] circuitstatus,
        [StringValue("stream-status")] streamstatus,
        [StringValue("orconn-status")] orconnstatus,
        [StringValue("entry-guards")] entryguards,
        [StringValue("traffic/read")] trafficread,
        [StringValue("traffic/written")] trafficwritten,
        [StringValue("accounting/enabled")] accountingenabled,

        [StringValue("accounting/hibernating")]
        accountinghibernating,
        [StringValue("accounting/bytes")] accountingbytes,
        [StringValue("accounting/bytes-left")] accountingbytesleft,

        [StringValue("accounting/interval-start")]
        accountingintervalstart,

        [StringValue("accounting/interval-wake")]
        accountingintervalwake,

        [StringValue("accounting/interval-end")]
        accountingintervalend,
        [StringValue("config/names")] confignames,
        [StringValue("config/defaults")] configdefaults,
        [StringValue("info/names")] infonames,
        [StringValue("events/names")] eventsnames,
        [StringValue("features/names")] featuresnames,
        [StringValue("signal/names")] signalnames,

        [StringValue("ip-to-country/ipv4-available")]
        iptocountryipv4available,

        [StringValue("ip-to-country/ipv6-available")]
        iptocountryipv6available,
        [StringValue("ip-to-country/*")] iptocountry,
        [StringValue("process/pid")] processpid,
        [StringValue("process/uid")] processuid,
        [StringValue("process/user")] processuser,

        [StringValue("process/descriptor-limit")]
        processdescriptorlimit,

        [StringValue("dir/status-vote/current/consensus")]
        dirstatusvotecurrentconsensus,
        [StringValue("dir/status/authority")] dirstatusauthority,
        [StringValue("dir/status/fp/<F>")] dirstatusfpF,

        [StringValue("dir/status/fp/<F1>+<F2>+<F3>")]
        dirstatusfpF1F2F3,
        [StringValue("dir/status/all")] dirstatusall,
        [StringValue("dir/server/fp/<F>")] dirserverfpF,

        [StringValue("dir/server/fp/<F1>+<F2>+<F3>")]
        dirserverfpF1F2F3,
        [StringValue("dir/server/d/<D>")] dirserverdD,

        [StringValue("dir/server/d/<D1>+<D2>+<D3>")]
        dirserverdD1D2D3,
        [StringValue("dir/server/authority")] dirserverauthority,
        [StringValue("dir/server/all")] dirserverall,

        [StringValue("status/circuit-established")]
        statuscircuitestablished,

        [StringValue("status/enough-dir-info")]
        statusenoughdirinfo,

        [StringValue("status/good-server-descriptor")]
        statusgoodserverdescriptor,

        [StringValue("status/accepted-server-descriptor")]
        statusacceptedserverdescriptor,
        [StringValue("status/...")] status,

        [StringValue("status/reachability-succeeded/or")]
        statusreachabilitysucceededor,

        [StringValue("status/reachability-succeeded/dir")]
        statusreachabilitysucceededdir,

        [StringValue("status/reachability-succeeded")]
        statusreachabilitysucceeded,

        [StringValue("status/bootstrap-phase")]
        statusbootstrapphase,

        [StringValue("status/version/recommended")]
        statusversionrecommended,

        [StringValue("status/version/current")]
        statusversioncurrent,

        [StringValue("status/version/num-concurring")]
        statusversionnumconcurring,

        [StringValue("status/version/num-versioning")]
        statusversionnumversioning,
        [StringValue("status/clients-seen")] statusclientsseen,

        [StringValue("status/fresh-relay-descs")]
        statusfreshrelaydescs,
        [StringValue("net/listeners/or")] netlistenersor,
        [StringValue("net/listeners/dir")] netlistenersdir,
        [StringValue("net/listeners/socks")] netlistenerssocks,
        [StringValue("net/listeners/trans")] netlistenerstrans,
        [StringValue("net/listeners/dir")] netlistenersnatd,
        [StringValue("net/listeners/dns")] netlistenersdns,
        [StringValue("net/listeners/control")] netlistenerscontrol,
        [StringValue("dir-usage")] dirusage,
        [StringValue("bw-event-cache")] bweventcache,
        [StringValue("consensus/valid-after")] consensusvalidafter,
        [StringValue("consensus/fresh-until")] consensusfreshuntil,
        [StringValue("hs/client/desc/id/")] hsclientdescidADDR,
        [StringValue("hs/service/desc/id/")] hsservicedescidADDR,
        [StringValue("onions/current")] onionscurrent,
        [StringValue("onions/detached")] onionsdetached,

        [StringValue("downloads/networkstatus/ns")]
        downloadsnetworkstatusns,

        [StringValue("downloads/networkstatus/ns/bootstrap")]
        downloadsnetworkstatusnsbootstrap,

        [StringValue("downloads/networkstatus/ns/running")]
        downloadsnetworkstatusnsrunning,

        [StringValue("downloads/networkstatus/microdesc")]
        downloadsnetworkstatusmicrodesc,

        [StringValue("downloads/networkstatus/microdesc/bootstrap")]
        downloadsnetworkstatusmicrodescbootstrap,

        [StringValue("downloads/networkstatus/microdesc/running")]
        downloadsnetworkstatusmicrodescrunning,
        [StringValue("downloads/cert/fps")] downloadscertfps,
        [StringValue("downloads/desc/descs")] downloadsdescdescs,
        [StringValue("downloads/desc/")] downloadsdescDigest,

        [StringValue("downloads/bridge/bridges")]
        downloadsbridgebridges,
        [StringValue("downloads/bridge/")] downloadsbridgeDigest,
        [StringValue("sr/current")] srcurrent,
        [StringValue("sr/previous")] srprevious,
        [StringValue("config-can-saveconf")] configcansaveconf
    }
}