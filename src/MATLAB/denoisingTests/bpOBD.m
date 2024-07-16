%function [reconImage, Projections, filteredImages] = bpOBD(origImages)

nProjections =length(origImages);

step=(length(origImages))/nProjections;

angleStep=360.0/(nProjections+1);

s=size(origImages{1});
sizeProjections = s(2)-99;

reconImage = zeros([sizeProjections sizeProjections]);


B=(sizeProjections/-2):(sizeProjections/2-1);
filter(1:length(B)) =abs(B);
filter=filter./(length(filter)/2)^2*3.14;
han=hann(length(filter))';
%filter=filter.*han;

filter=fftshift(filter);

addition = zeros([sizeProjections sizeProjections]);
for I=1:nProjections
    sI=round( (I-1)*step+1)
  
    sI2 =round( mod(sI+ 250-1,length(origImages)-1)+1)
    
    projection =doOBDFilter( origImages{sI}, flipud( origImages{sI2}));
    filteredImages{I}=projection;
    projection = projection(:,round(size(projection,2)/2))';
    
    Projections{I}=projection;
     
    if length(projection)~=length(filter)
        clear filter;
        sizeProjections=length(projection);
        reconImage = zeros([sizeProjections sizeProjections]);
        addition = zeros([sizeProjections sizeProjections]);
        B=(sizeProjections/-2):(sizeProjections/2-1);
        filter(1:length(B)) =abs(B);
        filter=filter./(length(filter)/2)^2*3.14;
        
    end
    
    fProjection =real(ifft(  fft(projection) .* filter ));
    
    
    for J=1:sizeProjections
       addition(:,J)= fProjection(J);
    end
    
    addition=imrotate(addition,angleStep*I,'nearest','crop');
    
    reconImage=reconImage + addition;
end

figure;imagesc(reconImage);colormap gray;
D=reconImage;
%end
