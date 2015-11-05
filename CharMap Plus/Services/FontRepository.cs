﻿using CharMap_Plus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.System.Threading;

namespace CharMap_Plus.Services
{
    public abstract class FontRepository
    {
        public Dictionary<string, string> UnicodeMap;

        public List<FontDetail> AllFonts { get; set; }

        public virtual Task RefreshAsync()
        {
            AllFonts = null;
            return LoadAsync();
        }


        public virtual Task LoadAsync()
        {
            if (AllFonts == null)
            {
                var allInstalledFonts = AllFonts = new List<FontDetail>();

                var enumer = new FontEnumeration.FontEnumerator();
                foreach (var f in enumer.ListSystemFonts().OrderBy(f => f))
                {
                    AllFonts.Add(new FontDetail() { Name = f, Type = "Installed", CharacterCount = 0 });
                }

                var backgroundTask = ThreadPool.RunAsync(async _ =>
                {
                    try
                    {
                        await Task.Delay(100);
                        Debug.WriteLine("start loading Unicodemap");
                        if (UnicodeMap == null)
                        {
                            var codesList = await GetFromFile<List<UnicodeChar>>("ms-appx:///Fonts/ucd.json");
                            UnicodeMap = codesList.ToDictionary(c => c.code, c => c.name);
                        }
                        Debug.WriteLine("loaded Unicodemap");

                        foreach (var f in allInstalledFonts)
                        {
                        // these throw error on mobile
                        if (f.Name == "Malgun Gothic") continue;
                        if (f.Name == "Microsoft JhengHei") continue;
                        if (f.Name == "Microsoft JhengHei UI") continue;
                        if (f.Name == "Microsoft YaHei") continue;
                        if (f.Name == "Microsoft YaHei UI") continue;
                        if (f.Name == "Yu Gothic") continue;
                        if (f.Name == "Yu Gothic UI") continue;

                        await ThreadPool.RunAsync(
                            w =>
                            {
                                var enumer2 = new FontEnumeration.FontEnumerator();
                                var codes = enumer2.ListSupportedChars(f.Name);
                                f.FontChars = codes.Where(c => c > 0 && c != 10 && c != 13 && c != 20).Select(c => new FontChar()
                                {
                                    Name = GetCharName(c),
                                    Char = (char)c,
                                    Family = f.Name,
                                    Size = 38
                                }).ToList();
                                f.CharacterCount = f.FontChars.Count;
                                Debug.WriteLine("loaded font chars for " + f.Name);
                            }, WorkItemPriority.Low);

                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }, WorkItemPriority.Low);
            }

            return Task.FromResult<object>(null);
        }

    
   public abstract List<FontGroup> GetFontGroups();

        public FontDetail GetFont(string name)
        {
            return AllFonts.FirstOrDefault(f => f.Name == name);
        }

        protected async Task<T> GetFromFile<T>(string fileUrl)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileUrl));
            var json = await FileIO.ReadTextAsync(file);
            var array = JsonConvert.DeserializeObject<T>(json);
            return array;
        }

        public string GetCharName(uint code)
        {
            var hex = code.ToString("x").PadLeft(4, '0').ToUpper();
            string value;
            UnicodeMap.TryGetValue(hex, out value);
            return value ?? "Personal Use";
        }
    }
}









/*

.icon-drawing-undo-point:before {
content: "\e025";
}
.icon-drawing-redo:before {
content: "\e026";
}
.icon-drawing-undo:before {
content: "\e027";
}
.icon-drawing-redo-curve:before {
content: "\e028";
}
.icon-drawing-undo-curve:before {
content: "\e029";
}
.icon-drawing-transform-rotate-right:before {
content: "\e02a";
}
.icon-drawing-transform-rotate-left:before {
content: "\e02b";
}
.icon-drawing-transform-rotate-counterclockwise:before {
content: "\e02c";
}
.icon-drawing-transform-rotate-clockwise:before {
content: "\e02d";
}
.icon-drawing-transform-flip-vertical:before {
content: "\e02e";
}
.icon-drawing-transform-flip-horizontal:before {
content: "\e02f";
}
.icon-drawing-tiles-plus:before {
content: "\e030";
}
.icon-drawing-tiles-minus:before {
content: "\e031";
}
.icon-drawing-tiles-four:before {
content: "\e032";
}
.icon-drawing-tiles-nine:before {
content: "\e033";
}
.icon-drawing-tiles-sixteen:before {
content: "\e034";
}
.icon-drawing-text-bold:before {
content: "\e035";
}
.icon-drawing-text-italic:before {
content: "\e036";
}
.icon-drawing-text-underline:before {
content: "\e037";
}
.icon-drawing-text-strikethrough:before {
content: "\e038";
}
.icon-drawing-text-overline:before {
content: "\e039";
}
.icon-drawing-text-size:before {
content: "\e03a";
}
.icon-drawing-text-size-plus:before {
content: "\e03b";
}
.icon-drawing-text-size-minus:before {
content: "\e03c";
}
.icon-drawing-text-size-up:before {
content: "\e03d";
}
.icon-drawing-text-size-down:before {
content: "\e03e";
}
.icon-drawing-text-serif:before {
content: "\e03f";
}
.icon-drawing-text-sans:before {
content: "\e040";
}
.icon-drawing-text-script:before {
content: "\e041";
}
.icon-drawing-text-align-left:before {
content: "\e042";
}
.icon-drawing-text-align-center:before {
content: "\e043";
}
.icon-drawing-text-align-right:before {
content: "\e044";
}
.icon-drawing-text-align-justify:before {
content: "\e045";
}
.icon-drawing-crop:before {
content: "\e046";
}
.icon-drawing-crosshair:before {
content: "\e047";
}
.icon-drawing-dimension-line-width-short:before {
content: "\e048";
}
.icon-drawing-dimension-line-height:before {
content: "\e049";
}
.icon-drawing-dimension-line-height-short:before {
content: "\e04a";
}
.icon-drawing-dimension-box-width:before {
content: "\e04b";
}
.icon-drawing-dimension-box-height:before {
content: "\e04c";
}
.icon-drawing-dimension-arrow-line-width-thick:before {
content: "\e04d";
}
.icon-drawing-dimension-arrow-line-width:before {
content: "\e04e";
}
.icon-drawing-dimension-arrow-line-width-short:before {
content: "\e04f";
}
.icon-drawing-dimension-arrow-line-height-thick:before {
content: "\e050";
}
.icon-drawing-dimension-arrow-line-height:before {
content: "\e051";
}
.icon-drawing-dimension-arrow-line-height-short:before {
content: "\e052";
}
.icon-drawing-dimension-arrow-box-width:before {
content: "\e053";
}
.icon-drawing-dimension-arrow-box-height:before {
content: "\e054";
}
.icon-drawing-dimension-line-width:before {
content: "\e055";
}
.icon-drawing-layer-arrange-solid-sendtoback:before {
content: "\e056";
}
.icon-drawing-layer-arrange-solid-bringtofront:before {
content: "\e057";
}
.icon-drawing-layer-arrange-solid-sendbackward:before {
content: "\e058";
}
.icon-drawing-layer-arrange-solid-bringforward:before {
content: "\e059";
}
.icon-drawing-align-left:before {
content: "\e05a";
}
.icon-drawing-align-center:before {
content: "\e05b";
}
.icon-drawing-align-right:before {
content: "\e05c";
}
.icon-drawing-align-justify:before {
content: "\e05d";
}
.icon-drawing-printer:before {
content: "\e05e";
}
.icon-drawing-printer-text:before {
content: "\e05f";
}
.icon-drawing-printer-blank:before {
content: "\e060";
}
.icon-drawing-layer:before {
content: "\e061";
}
.icon-drawing-layer-down:before {
content: "\e062";
}
.icon-drawing-layer-up:before {
content: "\e063";
}
.icon-drawing-layer-add:before {
content: "\e064";
}
.icon-drawing-layer-minus:before {
content: "\e065";
}
.icon-drawing-layer-delete:before {
content: "\e066";
}
.icon-drawing-image-export:before {
content: "\e067";
}
.icon-drawing-image-gallery:before {
content: "\e068";
}
.icon-drawing-list-one:before {
content: "\e069";
}
.icon-drawing-list-two:before {
content: "\e06a";
}
.icon-drawing-list:before {
content: "\e06b";
}
.icon-drawing-list-create:before {
content: "\e06c";
}
.icon-drawing-list-star:before {
content: "\e06d";
}
.icon-drawing-list-gear:before {
content: "\e06f";
}
.icon-drawing-list-select:before {
content: "\e070";
}
.icon-drawing-list-reorder-down:before {
content: "\e076";
}
.icon-drawing-list-reorder:before {
content: "\e077";
}
.icon-drawing-list-reorder-up:before {
content: "\e078";
}
.icon-drawing-list-hidden:before {
content: "\e079";
}
.icon-drawing-list-merge:before {
content: "\e07a";
}
.icon-drawing-section-expand-all:before {
content: "\e07b";
}
.icon-drawing-section-collapse-all:before {
content: "\e07c";
}
.icon-drawing-section-expand:before {
content: "\e07d";
}
.icon-drawing-section-collapse:before {
content: "\e07e";
}
.icon-drawing-checkmark-cross:before {
content: "\e07f";
}
.icon-drawing-checkmark-uncrossed:before {
content: "\e080";
}
.icon-drawing-folder-open:before {
content: "\e081";
}
.icon-drawing-folder:before {
content: "\e082";
}
.icon-drawing-folder-lock:before {
content: "\e083";
}
.icon-drawing-folder-ellipsis:before {
content: "\e084";
}
.icon-drawing-appbarcursordefault:before {
content: "\e06e";
}
.icon-drawing-appbarcursordefaultoutline:before {
content: "\e071";
}
.icon-drawing-appbarcursorhand:before {
content: "\e072";
}*/






/*
        $hexes: 'f101' 'f102' 'f103' 'f104' 'f105' 'f106' 'f107' 'f108' 'f109' 'f10a' 'f10b' 'f10c' 'f10d' 'f10e' 'f10f' 'f110' 'f111' 'f112' 'f113' 'f114' 'f115' 'f116' 'f117' 'f118' 'f119' 'f11a' 'f11b' 'f11c' 'f11d' 'f11e' 'f11f' 'f120' 'f121' 'f122' 'f123' 'f124' 'f125' 'f126' 'f127' 'f128' 'f129' 'f12a' 'f12b' 'f12c' 'f12d' 'f12e' 'f12f' 'f130' 'f131' 'f132' 'f133' 'f134' 'f135' 'f136' 'f137' 'f138' 'f139' 'f13a' 'f13b' 'f13c' 'f13d' 'f13e' 'f13f' 'f140' 'f141' 'f142' 'f143' 'f144' 'f145' 'f146' 'f147' 'f148' 'f149' 'f14a' 'f14b' 'f14c' 'f14d' 'f14e' 'f14f' 'f150' 'f151' 'f152' 'f153' 'f154' 'f155' 'f156' 'f157' 'f158' 'f159' 'f15a' 'f15b' 'f15c' 'f15d' 'f15e' 'f15f' 'f160' 'f161' 'f162' 'f163' 'f164' 'f165' 'f166' 'f167' 'f168' 'f169' 'f16a' 'f16b' 'f16c' 'f16d' 'f16e' 'f16f' 'f170' 'f171' 'f172' 'f173' 'f174' 'f175' 'f176' 'f177' 'f178' 'f179' 'f17a' 'f17b' 'f17c' 'f17d' 'f17e' 'f17f' 'f180' 'f181' 'f182' 'f183' 'f184' 'f185' 'f186' 'f187' 'f188' 'f189' 'f18a' 'f18b' 'f18c' 'f18d' 'f18e' 'f18f' 'f190' 'f191' 'f192' 'f193' 'f194' 'f195' 'f196' 'f197' 'f198' 'f199' 'f19a' 'f19b' 'f19c' 'f19d' 'f19e' 'f19f' 'f1a0' 'f1a1' 'f1a2' 'f1a3' 'f1a4' 'f1a5' 'f1a6' 'f1a7' 'f1a8' 'f1a9' 'f1aa' 'f1ab' 'f1ac' 'f1ad' 'f1ae' 'f1af' 'f1b0' 'f1b1' 'f1b2' 'f1b3' 'f1b4' 'f1b5' 'f1b6' 'f1b7' 'f1b8' 'f1b9' 'f1ba' 'f1bb' 'f1bc' 'f1bd' 'f1be' 'f1bf' 'f1c0' 'f1c1' 'f1c2' 'f1c3' 'f1c4' 'f1c5' 'f1c6' 'f1c7' 'f1c8' 'f1c9' 'f1ca' 'f1cb' 'f1cc' 'f1cd' 'f1ce' 'f1cf' 'f1d0' 'f1d1' 'f1d2' 'f1d3' 'f1d4' 'f1d5' 'f1d6' 'f1d7' 'f1d8' 'f1d9' 'f1da' 'f1db' 'f1dc' 'f1dd' 'f1de' 'f1df' 'f1e0' 'f1e1' 'f1e2' 'f1e3' 'f1e4' 'f1e5' 'f1e6' 'f1e7' 'f1e8' 'f1e9' 'f1ea' 'f1eb' 'f1ec' 'f1ed' 'f1ee' 'f1ef' 'f1f0' 'f1f1' 'f1f2' 'f1f3' 'f1f4' 'f1f5' 'f1f6' 'f1f7' 'f1f8' 'f1f9' 'f1fa' 'f1fb' 'f1fc' 'f1fd' 'f1fe' 'f1ff' 'f200' 'f201' 'f202' 'f203' 'f204' 'f205' 'f206' 'f207' 'f208' 'f209' 'f20a' 'f20b' 'f20c' 'f20d' 'f20e' 'f20f' 'f210' 'f211' 'f212' 'f213' 'f214' 'f215' 'f216' 'f217' 'f218' 'f219' 'f21a' 'f21b' 'f21c' 'f21d' 'f21e' 'f21f' 'f220' 'f221' 'f222' 'f223' 'f224' 'f225' 'f226' 'f227' 'f228' 'f229' 'f22a' 'f22b' 'f22c' 'f22d' 'f22e' 'f22f' 'f230' 'f231' 'f232' 'f233' 'f234' 'f235' 'f236' 'f237' 'f238' 'f239' 'f23a' 'f23b' 'f23c' 'f23d' 'f23e' 'f23f' 'f240' 'f241' 'f242' 'f243' 'f244' 'f245' 'f246' 'f247' 'f248' 'f249' 'f24a' 'f24b' 'f24c' 'f24d' 'f24e' 'f24f' 'f250' 'f251' 'f252' 'f253' 'f254' 'f255' 'f256' 'f257' 'f258' 'f259' 'f25a' 'f25b' 'f25c' 'f25d' 'f25e' 'f25f' 'f260' 'f261' 'f262' 'f263' 'f264' 'f265' 'f266' 'f267' 'f268' 'f269' 'f26a' 'f26b' 'f26c' 'f26d' 'f26e' 'f26f' 'f270' 'f271' 'f272' 'f273' 'f274' 'f275' 'f276' 'f277' 'f278' 'f279' 'f27a' 'f27b' 'f27c' 'f27d' 'f27e' 'f27f' 'f280' 'f281' 'f282' 'f283' 'f284' 'f285' 'f286' 'f287' 'f288' 'f289' 'f28a' 'f28b' 'f28c' 'f28d' 'f28e' 'f28f' 'f290' 'f291' 'f292' 'f293' 'f294' 'f295' 'f296' 'f297' 'f298' 'f299' 'f29a' 'f29b' 'f29c' 'f29d' 'f29e' 'f29f' 'f2a0' 'f2a1' 'f2a2' 'f2a3' 'f2a4' 'f2a5' 'f2a6' 'f2a7' 'f2a8' 'f2a9' 'f2aa' 'f2ab' 'f2ac' 'f2ad' 'f2ae' 'f2af' 'f2b0' 'f2b1' 'f2b2' 'f2b3' 'f2b4' 'f2b5' 'f2b6' 'f2b7' 'f2b8' 'f2b9' 'f2ba' 'f2bb' 'f2bc' 'f2bd' 'f2be' 'f2bf' 'f2c0' 'f2c1' 'f2c2' 'f2c3' 'f2c4' 'f2c5' 'f2c6' 'f2c7' 'f2c8' 'f2c9' 'f2ca' 'f2cb' 'f2cc' 'f2cd' 'f2ce' 'f2cf' 'f2d0' 'f2d1' 'f2d2' 'f2d3' 'f2d4' 'f2d5' 'f2d6' 'f2d7' 'f2d8' 'f2d9' 'f2da' 'f2db' 'f2dc' 'f2dd' 'f2de' 'f2df' 'f2e0' 'f2e1' 'f2e2' 'f2e3' 'f2e4' 'f2e5' 'f2e6' 'f2e7' 'f2e8' 'f2e9' 'f2ea' 'f2eb' 'f2ec' 'f2ed' 'f2ee' 'f2ef' 'f2f0' 'f2f1' 'f2f2' 'f2f3' 'f2f4' 'f2f5' 'f2f6' 'f2f7' 'f2f8' 'f2f9' 'f2fa' 'f2fb' 'f2fc' 'f2fd' 'f2fe' 'f2ff' 'f300' 'f301' 'f302' 'f303' 'f304' 'f305' 'f306' 'f307' 'f308' 'f309' 'f30a' 'f30b' 'f30c' 'f30d' 'f30e' 'f30f' 'f310' 'f311' 'f312' 'f313' 'f314' 'f315' 'f316' 'f317' 'f318' 'f319' 'f31a' 'f31b' 'f31c' 'f31d' 'f31e' 'f31f' 'f320' 'f321' 'f322' 'f323' 'f324' 'f325' 'f326' 'f327' 'f328' 'f329' 'f32a' 'f32b' 'f32c' 'f32d' 'f32e' 'f32f' 'f330' 'f331' 'f332' 'f333' 'f334' 'f335' 'f336' 'f337' 'f338' 'f339' 'f33a' 'f33b' 'f33c' 'f33d' 'f33e' 'f33f' 'f340' 'f341' 'f342' 'f343' 'f344' 'f345' 'f346' 'f347' 'f348' 'f349' 'f34a' 'f34b' 'f34c' 'f34d' 'f34e' 'f34f' 'f350' 'f351' 'f352' 'f353' 'f354' 'f355' 'f356' 'f357' 'f358' 'f359' 'f35a' 'f35b' 'f35c' 'f35d' 'f35e' 'f35f' 'f360' 'f361' 'f362' 'f363' 'f364' 'f365' 'f366' 'f367' 'f368' 'f369' 'f36a' 'f36b' 'f36c' 'f36d' 'f36e' 'f36f' 'f370' 'f371' 'f372' 'f373' 'f374' 'f375' 'f376' 'f377' 'f378' 'f379' 'f37a' 'f37b' 'f37c' 'f37d' 'f37e' 'f37f' 'f380' 'f381' 'f382' 'f383' 'f384' 'f385' 'f386' 'f387' 'f388' 'f389' 'f38a' 'f38b' 'f38c' 'f38d' 'f38e' 'f38f' 'f390' 'f391' 'f392' 'f393' 'f394' 'f395' 'f396' 'f397' 'f398' 'f399' 'f39a' 'f39b' 'f39c' 'f39d' 'f39e' 'f39f' 'f3a0' 'f3a1' 'f3a2' 'f3a3' 'f3a4' 'f3a5' 'f3a6' 'f3a7' 'f3a8' 'f3a9' 'f3aa' 'f3ab' 'f3ac' 'f3ad' 'f3ae' 'f3af' 'f3b0' 'f3b1' 'f3b2' 'f3b3' 'f3b4' 'f3b5' 'f3b6' 'f3b7' 'f3b8' 'f3b9' 'f3ba' 'f3bb' 'f3bc' 'f3bd' 'f3be' 'f3bf' 'f3c0' 'f3c1' 'f3c2' 'f3c3' 'f3c4' 'f3c5' 'f3c6' 'f3c7' 'f3c8' 'f3c9' 'f3ca' 'f3cb' 'f3cc' 'f3cd' 'f3ce' 'f3cf' 'f3d0' 'f3d1' 'f3d2' 'f3d3' 'f3d4' 'f3d5' 'f3d6' 'f3d7' 'f3d8' 'f3d9' 'f3da' 'f3db' 'f3dc' 'f3dd' 'f3de' 'f3df' 'f3e0' 'f3e1' 'f3e2' 'f3e3' 'f3e4' 'f3e5' 'f3e6' 'f3e7' 'f3e8' 'f3e9' 'f3ea' 'f3eb' 'f3ec' 'f3ed' 'f3ee' 'f3ef' 'f3f0' 'f3f1' 'f3f2' 'f3f3' 'f3f4' 'f3f5' 'f3f6' 'f3f7' 'f3f8' 'f3f9' 'f3fa' 'f3fb' 'f3fc' 'f3fd' 'f3fe' 'f3ff' 'f400' 'f401' 'f402' 'f403' 'f404' 'f405' 'f406' 'f407' 'f408' 'f409' 'f40a' 'f40b' 'f40c' 'f40d' 'f40e' 'f40f' 'f410' 'f411' 'f412' 'f413' 'f414' 'f415' 'f416' 'f417' 'f418' 'f419' 'f41a' 'f41b' 'f41c' 'f41d' 'f41e' 'f41f' 'f420' 'f421' 'f422' 'f423' 'f424' 'f425' 'f426' 'f427' 'f428' 'f429' 'f42a' 'f42b' 'f42c' 'f42d' 'f42e' 'f42f' 'f430' 'f431' 'f432' 'f433' 'f434' 'f435' 'f436' 'f437' 'f438' 'f439' 'f43a' 'f43b' 'f43c' 'f43d' 'f43e' 'f43f' 'f440' 'f441' 'f442' 'f443' 'f444' 'f445' 'f446' 'f447' 'f448' 'f449' 'f44a' 'f44b' 'f44c' 'f44d' 'f44e' 'f44f' 'f450' 'f451' 'f452' 'f453' 'f454' 'f455' 'f456' 'f457' 'f458' 'f459' 'f45a' 'f45b' 'f45c' 'f45d' 'f45e' 'f45f' 'f460' 'f461' 'f462' 'f463' 'f464' 'f465' 'f466' 'f467' 'f468' 'f469' 'f46a' 'f46b' 'f46c' 'f46d' 'f46e' 'f46f' 'f470' 'f471' 'f472' 'f473' 'f474' 'f475' 'f476' 'f477' 'f478' 'f479' 'f47a' 'f47b' 'f47c' 'f47d' 'f47e' 'f47f' 'f480' 'f481' 'f482' 'f483' 'f484' 'f485' 'f486' 'f487' 'f488' 'f489' 'f48a' 'f48b' 'f48c' 'f48d' 'f48e' 'f48f' 'f490' 'f491' 'f492' 'f493' 'f494' 'f495' 'f496' 'f497' 'f498' 'f499' 'f49a' 'f49b' 'f49c' 'f49d' 'f49e' 'f49f' 'f4a0' 'f4a1' 'f4a2' 'f4a3' 'f4a4' 'f4a5' 'f4a6' 'f4a7' 'f4a8' 'f4a9' 'f4aa' 'f4ab' 'f4ac' 'f4ad' 'f4ae' 'f4af' 'f4b0' 'f4b1' 'f4b2' 'f4b3' 'f4b4' 'f4b5' 'f4b6' 'f4b7' 'f4b8' 'f4b9' 'f4ba' 'f4bb' 'f4bc' 'f4bd' 'f4be' 'f4bf' 'f4c0' 'f4c1' 'f4c2' 'f4c3' 'f4c4' 'f4c5' 'f4c6' 'f4c7' 'f4c8' 'f4c9' 'f4ca' 'f4cb' 'f4cc' 'f4cd' 'f4ce' 'f4cf' 'f4d0' 'f4d1' 'f4d2' 'f4d3' 'f4d4' 'f4d5' 'f4d6' 'f4d7' 'f4d8' 'f4d9' 'f4da' 'f4db' 'f4dc' 'f4dd' 'f4de' 'f4df' 'f4e0' 'f4e1' 'f4e2' 'f4e3' 'f4e4' 'f4e5' 'f4e6' 'f4e7' 'f4e8' 'f4e9' 'f4ea' 'f4eb' 'f4ec' 'f4ed' 'f4ee' 'f4ef' 'f4f0' 'f4f1' 'f4f2' 'f4f3' 'f4f4' 'f4f5' 'f4f6' 'f4f7' 'f4f8' 'f4f9' 'f4fa' 'f4fb' 'f4fc' 'f4fd' 'f4fe' 'f4ff' 'f500' 'f501' 'f502' 'f503' 'f504' 'f505' 'f506' 'f507' 'f508' 'f509' 'f50a' 'f50b' 'f50c' 'f50d' 'f50e' 'f50f' 'f510' 'f511' 'f512' 'f513' 'f514' 'f515' 'f516' 'f517' 'f518' 'f519' 'f51a' 'f51b' 'f51c' 'f51d' 'f51e' 'f51f' 'f520' 'f521' 'f522' 'f523' 'f524' 'f525' 'f526' 'f527' 'f528' 'f529' 'f52a' 'f52b' 'f52c' 'f52d' 'f52e' 'f52f' 'f530' 'f531' 'f532' 'f533' 'f534' 'f535' 'f536' 'f537' 'f538' 'f539' 'f53a' 'f53b' 'f53c' 'f53d' 'f53e' 'f53f' 'f540' 'f541' 'f542' 'f543' 'f544' 'f545' 'f546' 'f547' 'f548' 'f549' 'f54a' 'f54b' 'f54c' 'f54d' 'f54e' 'f54f' 'f550' 'f551' 'f552' 'f553' 'f554' 'f555' 'f556' 'f557' 'f558' 'f559' 'f55a' 'f55b' 'f55c' 'f55d' 'f55e' 'f55f' 'f560' 'f561' 'f562' 'f563' 'f564' 'f565' 'f566' 'f567' 'f568' 'f569' 'f56a' 'f56b' 'f56c' 'f56d' 'f56e' 'f56f' 'f570' 'f571' 'f572' 'f573' 'f574' 'f575' 'f576' 'f577' 'f578' 'f579' 'f57a' 'f57b' 'f57c' 'f57d' 'f57e' 'f57f' 'f580' 'f581' 'f582' 'f583' 'f584' 'f585' 'f586' 'f587' 'f588' 'f589' 'f58a' 'f58b' 'f58c' 'f58d' 'f58e' 'f58f' 'f590' 'f591' 'f592' 'f593' 'f594' 'f595' 'f596' 'f597' 'f598' 'f599' 'f59a' 'f59b' 'f59c' 'f59d' 'f59e' 'f59f' 'f5a0' 'f5a1' 'f5a2' 'f5a3' 'f5a4' 'f5a5' 'f5a6' 'f5a7' 'f5a8' 'f5a9' 'f5aa' 'f5ab' 'f5ac' 'f5ad' 'f5ae' 'f5af' 'f5b0' 'f5b1' 'f5b2' 'f5b3' 'f5b4' 'f5b5' 'f5b6' 'f5b7' 'f5b8' 'f5b9' 'f5ba' 'f5bb' 'f5bc' 'f5bd' 'f5be' 'f5bf' 'f5c0' 'f5c1' 'f5c2' 'f5c3' 'f5c4' 'f5c5' 'f5c6' 'f5c7' 'f5c8' 'f5c9' 'f5ca' 'f5cb' 'f5cc' 'f5cd' 'f5ce' 'f5cf' 'f5d0' 'f5d1' 'f5d2' 'f5d3' 'f5d4' 'f5d5' 'f5d6' 'f5d7' 'f5d8' 'f5d9' 'f5da' 'f5db' 'f5dc' 'f5dd' 'f5de' 'f5df' 'f5e0' 'f5e1' 'f5e2' 'f5e3' 'f5e4' 'f5e5' 'f5e6' 'f5e7' 'f5e8' 'f5e9' 'f5ea' 'f5eb' 'f5ec' 'f5ed' 'f5ee' 'f5ef' 'f5f0';
        $names: 'account' 'account-alert' 'account-box' 'account-box-outline' 'account-check' 'account-circle' 'account-key' 'account-location' 'account-minus' 'account-multiple' 'account-multiple-outline' 'account-multiple-plus' 'account-network' 'account-outline' 'account-plus' 'account-remove' 'account-search' 'account-star' 'account-star-variant' 'account-switch' 'airballoon' 'airplane' 'airplane-off' 'alarm' 'alarm-check' 'alarm-multiple' 'alarm-off' 'alarm-plus' 'album' 'alert' 'alert-box' 'alert-circle' 'alert-octagon' 'alpha' 'alphabetical' 'amazon' 'amazon-clouddrive' 'ambulance' 'android' 'android-debug-bridge' 'android-studio' 'apple' 'apple-finder' 'apple-ios' 'apple-mobileme' 'apple-safari' 'appnet' 'apps' 'archive' 'arrange-bring-forward' 'arrange-bring-to-front' 'arrange-send-backward' 'arrange-send-to-back' 'arrow-all' 'arrow-bottom-left' 'arrow-bottom-right' 'arrow-collapse' 'arrow-down' 'arrow-down-bold' 'arrow-down-bold-circle' 'arrow-down-bold-circle-outline' 'arrow-down-bold-hexagon-outline' 'arrow-expand' 'arrow-left' 'arrow-left-bold' 'arrow-left-bold-circle' 'arrow-left-bold-circle-outline' 'arrow-left-bold-hexagon-outline' 'arrow-right' 'arrow-right-bold' 'arrow-right-bold-circle' 'arrow-right-bold-circle-outline' 'arrow-right-bold-hexagon-outline' 'arrow-top-left' 'arrow-top-right' 'arrow-up' 'arrow-up-bold' 'arrow-up-bold-circle' 'arrow-up-bold-circle-outline' 'arrow-up-bold-hexagon-outline' 'at' 'attachment' 'audiobook' 'auto-fix' 'auto-upload' 'baby' 'backburger' 'backup-restore' 'bank' 'barcode' 'barley' 'barrel' 'basecamp' 'basket' 'basket-fill' 'basket-unfill' 'battery' 'battery-10' 'battery-20' 'battery-30' 'battery-40' 'battery-50' 'battery-60' 'battery-70' 'battery-80' 'battery-90' 'battery-alert' 'battery-charging-100' 'battery-charging-20' 'battery-charging-30' 'battery-charging-40' 'battery-charging-60' 'battery-charging-80' 'battery-charging-90' 'battery-minus' 'battery-negative' 'battery-outline' 'battery-plus' 'battery-positive' 'battery-unknown' 'beach' 'beaker' 'beaker-empty' 'beaker-empty-outline' 'beaker-outline' 'beats' 'beer' 'behance' 'bell' 'bell-off' 'bell-outline' 'bell-ring' 'bell-ring-outline' 'bell-sleep' 'beta' 'bike' 'bing' 'binoculars' 'bio' 'biohazard' 'bitbucket' 'black-mesa' 'blackberry' 'blinds' 'block-helper' 'blogger' 'bluetooth' 'bluetooth-audio' 'bluetooth-connect' 'bluetooth-settings' 'bluetooth-transfer' 'blur' 'blur-linear' 'blur-off' 'blur-radial' 'bone' 'book' 'book-multiple' 'book-multiple-variant' 'book-open' 'book-variant' 'bookmark' 'bookmark-check' 'bookmark-music' 'bookmark-outline' 'bookmark-outline-plus' 'bookmark-plus' 'bookmark-remove' 'border-all' 'border-bottom' 'border-color' 'border-horizontal' 'border-inside' 'border-left' 'border-none' 'border-outside' 'border-right' 'border-top' 'border-vertical' 'bowling' 'box' 'briefcase' 'briefcase-check' 'briefcase-download' 'briefcase-upload' 'brightness-1' 'brightness-2' 'brightness-3' 'brightness-4' 'brightness-5' 'brightness-6' 'brightness-7' 'brightness-auto' 'broom' 'brush' 'bug' 'bulletin-board' 'bullhorn' 'bus' 'cake' 'cake-variant' 'calculator' 'calendar' 'calendar-blank' 'calendar-check' 'calendar-clock' 'calendar-multiple' 'calendar-multiple-check' 'calendar-plus' 'calendar-remove' 'calendar-text' 'calendar-today' 'camcorder' 'camcorder-box' 'camcorder-box-off' 'camcorder-off' 'camera' 'camera-front' 'camera-front-variant' 'camera-iris' 'camera-party-mode' 'camera-rear' 'camera-rear-variant' 'camera-switch' 'camera-timer' 'candycane' 'car' 'car-wash' 'carrot' 'cart' 'cart-outline' 'cash' 'cash-100' 'cash-multiple' 'cash-usd' 'cast' 'cast-connected' 'castle' 'cat' 'cellphone' 'cellphone-android' 'cellphone-dock' 'cellphone-iphone' 'cellphone-link' 'cellphone-link-off' 'cellphone-settings' 'chair-school' 'chart-arc' 'chart-areaspline' 'chart-bar' 'chart-histogram' 'chart-line' 'chart-pie' 'check' 'check-all' 'checkbox-blank' 'checkbox-blank-circle' 'checkbox-blank-circle-outline' 'checkbox-blank-outline' 'checkbox-marked' 'checkbox-marked-circle' 'checkbox-marked-circle-outline' 'checkbox-marked-outline' 'checkbox-multiple-blank' 'checkbox-multiple-blank-outline' 'checkbox-multiple-marked' 'checkbox-multiple-marked-outline' 'checkerboard' 'chevron-double-down' 'chevron-double-left' 'chevron-double-right' 'chevron-double-up' 'chevron-down' 'chevron-left' 'chevron-right' 'chevron-up' 'church' 'cisco-webex' 'city' 'clipboard' 'clipboard-account' 'clipboard-alert' 'clipboard-arrow-down' 'clipboard-arrow-left' 'clipboard-check' 'clipboard-outline' 'clipboard-text' 'clippy' 'clock' 'clock-fast' 'close' 'close-box' 'close-box-outline' 'close-circle' 'close-circle-outline' 'close-network' 'closed-caption' 'cloud' 'cloud-check' 'cloud-circle' 'cloud-download' 'cloud-outline' 'cloud-outline-off' 'cloud-upload' 'code-array' 'code-braces' 'code-equal' 'code-greater-than' 'code-less-than' 'code-less-than-or-equal' 'code-not-equal' 'code-not-equal-variant' 'code-string' 'code-tags' 'codepen' 'coffee' 'coffee-to-go' 'coin' 'color-helper' 'comment' 'comment-account' 'comment-account-outline' 'comment-alert' 'comment-alert-outline' 'comment-check' 'comment-check-outline' 'comment-multiple-outline' 'comment-outline' 'comment-plus-outline' 'comment-processing' 'comment-processing-outline' 'comment-remove-outline' 'comment-text' 'comment-text-outline' 'compare' 'compass' 'compass-outline' 'console' 'content-copy' 'content-cut' 'content-duplicate' 'content-paste' 'content-save' 'content-save-all' 'contrast' 'contrast-box' 'contrast-circle' 'cow' 'credit-card' 'credit-card-multiple' 'crop' 'crop-free' 'crop-landscape' 'crop-portrait' 'crop-square' 'crosshairs' 'crosshairs-gps' 'crown' 'cube' 'cube-outline' 'cube-unfolded' 'cup' 'cup-water' 'currency-btc' 'currency-eur' 'currency-gbp' 'currency-inr' 'currency-rub' 'currency-try' 'currency-usd' 'cursor-default' 'cursor-default-outline' 'cursor-move' 'cursor-pointer' 'database' 'database-minus' 'database-outline' 'database-plus' 'debug-step-into' 'debug-step-out' 'debug-step-over' 'decimal-decrease' 'decimal-increase' 'delete' 'delete-variant' 'deskphone' 'desktop-mac' 'desktop-tower' 'details' 'deviantart' 'diamond' 'dice' 'dice-1' 'dice-2' 'dice-3' 'dice-4' 'dice-5' 'dice-6' 'directions' 'disk-alert' 'disqus' 'disqus-outline' 'division' 'division-box' 'dns' 'domain' 'dots-horizontal' 'dots-vertical' 'download' 'drag' 'drag-horizontal' 'drag-vertical' 'drawing' 'drawing-box' 'dribbble' 'dribbble-box' 'drone' 'dropbox' 'drupal' 'duck' 'dumbbell' 'earth' 'earth-off' 'edge' 'eject' 'elevation-decline' 'elevation-rise' 'elevator' 'email' 'email-open' 'email-outline' 'email-secure' 'emoticon' 'emoticon-cool' 'emoticon-devil' 'emoticon-happy' 'emoticon-neutral' 'emoticon-poop' 'emoticon-sad' 'emoticon-tongue' 'engine' 'engine-outline' 'equal' 'equal-box' 'eraser' 'escalator' 'etsy' 'evernote' 'exclamation' 'exit-to-app' 'export' 'eye' 'eye-off' 'eyedropper' 'eyedropper-variant' 'facebook' 'facebook-box' 'facebook-messenger' 'factory' 'fan' 'fast-forward' 'ferry' 'file' 'file-cloud' 'file-delimited' 'file-document' 'file-document-box' 'file-excel' 'file-excel-box' 'file-find' 'file-image' 'file-image-box' 'file-multiple' 'file-music' 'file-outline' 'file-pdf' 'file-pdf-box' 'file-powerpoint' 'file-powerpoint-box' 'file-presentation-box' 'file-video' 'file-word' 'file-word-box' 'file-xml' 'film' 'filmstrip' 'filmstrip-off' 'filter' 'filter-outline' 'filter-remove' 'filter-remove-outline' 'filter-variant' 'fire' 'firefox' 'fish' 'flag' 'flag-checkered' 'flag-outline' 'flag-outline-variant' 'flag-triangle' 'flag-variant' 'flash' 'flash-auto' 'flash-off' 'flashlight' 'flashlight-off' 'flattr' 'flip-to-back' 'flip-to-front' 'floppy' 'flower' 'folder' 'folder-account' 'folder-download' 'folder-google-drive' 'folder-image' 'folder-lock' 'folder-lock-open' 'folder-move' 'folder-multiple' 'folder-multiple-image' 'folder-multiple-outline' 'folder-outline' 'folder-plus' 'folder-remove' 'folder-upload' 'food' 'food-apple' 'food-variant' 'football' 'football-helmet' 'format-align-center' 'format-align-justify' 'format-align-left' 'format-align-right' 'format-bold' 'format-clear' 'format-color-fill' 'format-float-center' 'format-float-left' 'format-float-none' 'format-float-right' 'format-header-1' 'format-header-2' 'format-header-3' 'format-header-4' 'format-header-5' 'format-header-6' 'format-header-decrease' 'format-header-equal' 'format-header-increase' 'format-header-pound' 'format-indent-decrease' 'format-indent-increase' 'format-italic' 'format-line-spacing' 'format-list-bulleted' 'format-list-numbers' 'format-paint' 'format-paragraph' 'format-quote' 'format-size' 'format-strikethrough' 'format-subscript' 'format-superscript' 'format-text' 'format-textdirection-l-to-r' 'format-textdirection-r-to-l' 'format-underline' 'format-wrap-inline' 'format-wrap-square' 'format-wrap-tight' 'format-wrap-top-bottom' 'forum' 'forward' 'foursquare' 'fridge' 'fullscreen' 'fullscreen-exit' 'function' 'gamepad' 'gamepad-variant' 'gas-station' 'gavel' 'gender-female' 'gender-male' 'gender-male-female' 'gender-transgender' 'gift' 'git' 'github-box' 'github-circle' 'glass-flute' 'glass-mug' 'glass-stange' 'glass-tulip' 'glasses' 'gmail' 'google' 'google-chrome' 'google-circles' 'google-circles-communities' 'google-circles-extended' 'google-circles-group' 'google-controller' 'google-controller-off' 'google-drive' 'google-earth' 'google-glass' 'google-maps' 'google-pages' 'google-play' 'google-plus' 'google-plus-box' 'grid' 'grid-off' 'group' 'guitar' 'guitar-pick' 'guitar-pick-outline' 'hand-pointing-right' 'hanger' 'hangouts' 'harddisk' 'headphones' 'headphones-box' 'headphones-settings' 'headset' 'headset-dock' 'headset-off' 'heart' 'heart-box' 'heart-box-outline' 'heart-broken' 'heart-outline' 'help' 'help-circle' 'hexagon' 'hexagon-outline' 'history' 'hololens' 'home' 'home-modern' 'home-variant' 'hops' 'hospital' 'hospital-building' 'hospital-marker' 'hotel' 'houzz' 'houzz-box' 'human' 'human-child' 'human-male-female' 'image-album' 'image-area' 'image-area-close' 'image-broken' 'image-filter' 'image-filter-black-white' 'image-filter-center-focus' 'image-filter-drama' 'image-filter-frames' 'image-filter-hdr' 'image-filter-none' 'image-filter-tilt-shift' 'image-filter-vintage' 'import' 'inbox' 'information' 'information-outline' 'instagram' 'instapaper' 'internet-explorer' 'invert-colors' 'jira' 'jsfiddle' 'keg' 'key' 'key-change' 'key-minus' 'key-plus' 'key-remove' 'key-variant' 'keyboard' 'keyboard-backspace' 'keyboard-caps' 'keyboard-close' 'keyboard-off' 'keyboard-return' 'keyboard-tab' 'keyboard-variant' 'label' 'label-outline' 'language-csharp' 'language-css3' 'language-html5' 'language-javascript' 'language-python' 'language-python-text' 'laptop' 'laptop-chromebook' 'laptop-mac' 'laptop-windows' 'lastfm' 'launch' 'layers' 'layers-off' 'leaf' 'library' 'library-books' 'library-music' 'library-plus' 'lightbulb' 'lightbulb-outline' 'link' 'link-off' 'link-variant' 'link-variant-off' 'linkedin' 'linkedin-box' 'linux' 'lock' 'lock-open' 'lock-open-outline' 'lock-outline' 'login' 'logout' 'looks' 'loupe' 'lumx' 'magnet' 'magnet-on' 'magnify' 'magnify-minus' 'magnify-plus' 'mail-ru' 'map' 'map-marker' 'map-marker-circle' 'map-marker-multiple' 'map-marker-off' 'map-marker-radius' 'margin' 'markdown' 'marker-check' 'martini' 'material-ui' 'math-compass' 'maxcdn' 'medium' 'memory' 'menu' 'menu-down' 'menu-left' 'menu-right' 'menu-up' 'message' 'message-alert' 'message-draw' 'message-image' 'message-processing' 'message-reply' 'message-text' 'message-text-outline' 'message-video' 'microphone' 'microphone-off' 'microphone-outline' 'microphone-settings' 'microphone-variant' 'microphone-variant-off' 'minus' 'minus-box' 'minus-circle' 'minus-circle-outline' 'minus-network' 'monitor' 'monitor-multiple' 'more' 'motorbike' 'mouse' 'mouse-off' 'mouse-variant' 'mouse-variant-off' 'movie' 'multiplication' 'multiplication-box' 'music-box' 'music-box-outline' 'music-circle' 'music-note' 'music-note-eighth' 'music-note-half' 'music-note-off' 'music-note-quarter' 'music-note-sixteenth' 'music-note-whole' 'nature' 'nature-people' 'navigation' 'needle' 'nest-protect' 'nest-thermostat' 'newspaper' 'nfc' 'nfc-tap' 'nfc-variant' 'note' 'note-outline' 'note-text' 'numeric' 'numeric-0-box' 'numeric-0-box-multiple-outline' 'numeric-0-box-outline' 'numeric-1-box' 'numeric-1-box-multiple-outline' 'numeric-1-box-outline' 'numeric-2-box' 'numeric-2-box-multiple-outline' 'numeric-2-box-outline' 'numeric-3-box' 'numeric-3-box-multiple-outline' 'numeric-3-box-outline' 'numeric-4-box' 'numeric-4-box-multiple-outline' 'numeric-4-box-outline' 'numeric-5-box' 'numeric-5-box-multiple-outline' 'numeric-5-box-outline' 'numeric-6-box' 'numeric-6-box-multiple-outline' 'numeric-6-box-outline' 'numeric-7-box' 'numeric-7-box-multiple-outline' 'numeric-7-box-outline' 'numeric-8-box' 'numeric-8-box-multiple-outline' 'numeric-8-box-outline' 'numeric-9-box' 'numeric-9-box-multiple-outline' 'numeric-9-box-outline' 'numeric-9-plus-box' 'numeric-9-plus-box-multiple-outline' 'numeric-9-plus-box-outline' 'nutriton' 'odnoklassniki' 'office' 'oil' 'omega' 'onedrive' 'open-in-app' 'open-in-new' 'ornament' 'ornament-variant' 'outbox' 'owl' 'package' 'package-down' 'package-up' 'package-variant' 'package-variant-closed' 'palette' 'palette-advanced' 'panda' 'pandora' 'panorama' 'panorama-fisheye' 'panorama-horizontal' 'panorama-vertical' 'panorama-wide-angle' 'paper-cut-vertical' 'paperclip' 'parking' 'pause' 'pause-circle' 'pause-circle-outline' 'pause-octagon' 'pause-octagon-outline' 'paw' 'pen' 'pencil' 'pencil-box' 'pencil-box-outline' 'percent' 'pharmacy' 'phone' 'phone-bluetooth' 'phone-forward' 'phone-hangup' 'phone-in-talk' 'phone-incoming' 'phone-locked' 'phone-log' 'phone-missed' 'phone-outgoing' 'phone-paused' 'phone-settings' 'pig' 'pill' 'pin' 'pin-off' 'pine-tree' 'pine-tree-box' 'pinterest' 'pinterest-box' 'pizza' 'play' 'play-box-outline' 'play-circle' 'play-circle-outline' 'playlist-minus' 'playlist-plus' 'playstation' 'plus' 'plus-box' 'plus-circle' 'plus-circle-outline' 'plus-network' 'plus-one' 'pocket' 'poll' 'poll-box' 'polymer' 'popcorn' 'pound' 'pound-box' 'power' 'power-settings' 'power-socket' 'presentation' 'presentation-play' 'printer' 'printer-3d' 'pulse' 'puzzle' 'qrcode' 'quadcopter' 'quality-high' 'quicktime' 'radiator' 'radio' 'radio-tower' 'radioactive' 'radiobox-blank' 'radiobox-marked' 'raspberrypi' 'rdio' 'read' 'readability' 'receipt' 'recycle' 'redo' 'redo-variant' 'refresh' 'relative-scale' 'reload' 'remote' 'rename-box' 'repeat' 'repeat-off' 'repeat-once' 'replay' 'reply' 'reply-all' 'reproduction' 'resize-bottom-right' 'responsive' 'rewind' 'ribbon' 'road' 'rocket' 'rotate-3d' 'rotate-left' 'rotate-left-variant' 'rotate-right' 'rotate-right-variant' 'routes' 'rss' 'rss-box' 'ruler' 'run' 'sale' 'satellite' 'satellite-variant' 'scale' 'scale-bathroom' 'school' 'screen-rotation' 'screen-rotation-lock' 'script' 'sd' 'security' 'security-network' 'select' 'select-all' 'select-inverse' 'select-off' 'send' 'server' 'server-minus' 'server-network' 'server-network-off' 'server-off' 'server-plus' 'server-remove' 'server-security' 'settings' 'settings-box' 'shape-plus' 'share' 'share-variant' 'shield' 'shield-outline' 'shopping' 'shopping-music' 'shuffle' 'sigma' 'sign-caution' 'signal' 'silverware' 'silverware-fork' 'silverware-spoon' 'silverware-variant' 'sim-alert' 'sitemap' 'skip-next' 'skip-previous' 'skype' 'skype-business' 'sleep' 'sleep-off' 'smoking' 'smoking-off' 'snapchat' 'snowman' 'sofa' 'sort' 'sort-alphabetical' 'sort-ascending' 'sort-descending' 'sort-numeric' 'sort-variant' 'soundcloud' 'source-fork' 'source-pull' 'speaker' 'speaker-off' 'speedometer' 'spellcheck' 'spotify' 'spotlight' 'spotlight-beam' 'square-inc' 'square-inc-cash' 'stackoverflow' 'star' 'star-circle' 'star-half' 'star-outline' 'steam' 'stethoscope' 'stocking' 'stop' 'store' 'store-24-hour' 'stove' 'subway' 'sunglasses' 'swap-horizontal' 'swap-vertical' 'swim' 'sword' 'sync' 'sync-alert' 'sync-off' 'tab' 'tab-unselected' 'table' 'table-column-plus-after' 'table-column-plus-before' 'table-column-remove' 'table-column-width' 'table-edit' 'table-large' 'table-row-height' 'table-row-plus-after' 'table-row-plus-before' 'table-row-remove' 'tablet' 'tablet-android' 'tablet-ipad' 'tag' 'tag-faces' 'tag-multiple' 'tag-outline' 'tag-text-outline' 'taxi' 'teamviewer' 'telegram' 'television' 'television-guide' 'temperature-celsius' 'temperature-fahrenheit' 'temperature-kelvin' 'tennis' 'tent' 'terrain' 'text-to-speech' 'text-to-speech-off' 'texture' 'theater' 'theme-light-dark' 'thermometer' 'thermometer-lines' 'thumb-down' 'thumb-down-outline' 'thumb-up' 'thumb-up-outline' 'thumbs-up-down' 'ticket' 'ticket-account' 'tie' 'timelapse' 'timer' 'timer-10' 'timer-3' 'timer-off' 'timer-sand' 'timetable' 'toggle-switch' 'toggle-switch-off' 'tooltip' 'tooltip-edit' 'tooltip-image' 'tooltip-outline' 'tooltip-outline-plus' 'tooltip-text' 'tor' 'traffic-light' 'train' 'tram' 'transcribe' 'transcribe-close' 'transfer' 'tree' 'trello' 'trending-down' 'trending-neutral' 'trending-up' 'trophy' 'trophy-award' 'trophy-variant' 'truck' 'tshirt-crew' 'tshirt-v' 'tumblr' 'tumblr-reblog' 'twitch' 'twitter' 'twitter-box' 'twitter-circle' 'twitter-retweet' 'ubuntu' 'umbrella' 'umbrella-outline' 'undo' 'undo-variant' 'unfold-less' 'unfold-more' 'ungroup' 'untappd' 'upload' 'usb' 'vector-curve' 'vector-point' 'vector-square' 'verified' 'vibrate' 'video' 'video-off' 'video-switch' 'view-agenda' 'view-array' 'view-carousel' 'view-column' 'view-dashboard' 'view-day' 'view-grid' 'view-headline' 'view-list' 'view-module' 'view-quilt' 'view-stream' 'view-week' 'vimeo' 'vine' 'vk' 'vk-box' 'vk-circle' 'voicemail' 'volume-high' 'volume-low' 'volume-medium' 'volume-off' 'vpn' 'walk' 'wallet' 'wallet-giftcard' 'wallet-membership' 'wallet-travel' 'watch' 'watch-export' 'watch-import' 'water' 'water-off' 'water-pump' 'weather-cloudy' 'weather-fog' 'weather-hail' 'weather-lightning' 'weather-night' 'weather-partlycloudy' 'weather-pouring' 'weather-rainy' 'weather-snowy' 'weather-sunny' 'weather-sunset' 'weather-sunset-down' 'weather-sunset-up' 'weather-windy' 'weather-windy-variant' 'web' 'webcam' 'weight' 'weight-kilogram' 'whatsapp' 'wheelchair-accessibility' 'white-balance-auto' 'white-balance-incandescent' 'white-balance-irradescent' 'white-balance-sunny' 'wifi' 'wii' 'wikipedia' 'window-close' 'window-closed' 'window-maximize' 'window-minimize' 'window-open' 'window-restore' 'windows' 'wordpress' 'worker' 'wunderlist' 'xbox' 'xbox-controller' 'xbox-controller-off' 'xda' 'xml' 'yeast' 'yelp' 'youtube-play' 'zip-box';
        */
