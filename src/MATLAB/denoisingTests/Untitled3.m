NET.addAssembly('C:\\Development\\CellCT\\Tomography Simplified\\ReconstructCells\\ReconstructCells\\bin\\matlab\\ReconstructCells.dll');

lib = ReconstructCells.MainScript.GetLibraryForMatlab('C:\\temp\\sinogram');

count = lib.Count;



width =round( lib.Item(2).Width*2.1);
height =round( lib.Item(2).Height*2.1);

psf=[];
tpsf=[];
for I=0:count-1
    I1=I;
    I2=mod( I+1,count);
    I3=mod( I+250,count);
    I4=mod( I+251,count);
    
    h0 = double( lib.Item(I1).Data );
    h1 = double( lib.Item(I2).Data );
    h2 = flipud( double( lib.Item(I3).Data ));
    h3 = flipud( double( lib.Item(I4).Data ));

   % h0=imresize(h0,0.1,'lanczos3');
   % h1=imresize(h1,0.1,'lanczos3');
    
    %y = { h0 h1 h2 h3 };
    h0=h0-min(min(h0));
    h1=h1-min(min(h1));
    
    h2=h2-min(min(h2));
    h3=h3-min(min(h3));
    
    aH0=2*(abs(h0)+3/8).^.5;
    aH1=2*(abs(h1)+3/8).^.5;
    
    aH2=2*(abs(h2)+3/8).^.5;
    aH3=2*(abs(h3)+3/8).^.5;
    y = { aH0 aH2 };

    psf=[];
    [image tpsf] = DeconvolveImageOBD(y,true, psf);
    image = image(500:1100,500:1100);
    cleanedImages{I+1}=image;
    lib.SaveDataArray( I,'C:\\temp\\VisualizedBin\image.mbin', NET.convertArray((image/2).^2-3/8,'System.Single',size(image)));
 

    
    y = { aH0 aH1 aH2 aH3 };

    psf=[];
    [image tpsf] = DeconvolveImageOBD(y,true, psf);
    image = image(500:1100,500:1100);
    moreImages{I+1}=image;
    lib.SaveDataArray( I,'C:\\temp\\Vis2\image.mbin', NET.convertArray( (image/2).^2-3/8,'System.Single',size(image)));
 
    
    
%     y = { imresize(h0,.5,'box') imresize(h2,.5,'box') };
% 
%     psf=[];
%     [image tpsf] = DeconvolveImageOBD(y,false, psf);
%     PhantomImages{I+1}=image;
%     lib.SaveDataArray( I,'C:\\temp\\Phantom\image.mbin', NET.convertArray(image,'System.Single',size(image)));

end